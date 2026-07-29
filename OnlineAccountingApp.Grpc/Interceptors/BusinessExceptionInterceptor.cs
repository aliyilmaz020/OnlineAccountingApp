using Grpc.Core;
using Grpc.Core.Interceptors;
using OnlineAccountingApp.Domain.Exceptions;
using System.Text.Json;

namespace OnlineAccountingApp.Grpc.Interceptors;

/// <summary>
/// gRPC equivalent of WebApi's GlobalExceptionHandler/ApiResultFilter: translates
/// BusinessException/ValidationException into an RpcException with an appropriate status code,
/// carrying the AppErrorCode (and, for validation failures, the field errors) as trailing
/// metadata, since gRPC has no JSON response-body concept to put them in.
/// </summary>
public sealed class BusinessExceptionInterceptor(ILogger<BusinessExceptionInterceptor> logger) : Interceptor
{
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return await continuation(request, context);
        }
        catch (ValidationException vex)
        {
            Metadata trailers = [];
            trailers.Add("error-code", vex.ErrorCode);
            trailers.Add("errors-json", JsonSerializer.Serialize(vex.Errors));
            throw new RpcException(new Status(StatusCode.InvalidArgument, vex.Message), trailers);
        }
        catch (BusinessException bex)
        {
            Metadata trailers = [];
            trailers.Add("error-code", bex.ErrorCode);
            throw new RpcException(new Status(MapStatusCode(bex), bex.Message), trailers);
        }
        catch (RpcException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception in gRPC call {Method}", context.Method);
            throw new RpcException(new Status(StatusCode.Internal, "An unexpected error occurred."));
        }
    }

    private static StatusCode MapStatusCode(BusinessException bex)
    {
        // The tenant-context-missing case reads as "required context wasn't attached to the
        // call" rather than "the input was malformed", so it gets its own override instead of
        // following the generic 400 -> InvalidArgument mapping below.
        if (bex.ErrorCode == AppErrorCodes.Tenant.CompanyNotSpecified)
        {
            return StatusCode.FailedPrecondition;
        }

        return bex.HttpStatusCode switch
        {
            400 => StatusCode.InvalidArgument,
            401 => StatusCode.Unauthenticated,
            403 => StatusCode.PermissionDenied,
            404 => StatusCode.NotFound,
            409 => StatusCode.AlreadyExists,
            _ => StatusCode.Unknown
        };
    }
}
