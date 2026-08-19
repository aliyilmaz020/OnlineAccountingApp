export interface Company {
  id: string;
  name: string;
  address: string;
  identityNumber: string;
  taxDepartment: string;
  phoneNumber: string;
  email: string;
}

export interface CreateCompanyRequest {
  name: string;
  address: string;
  identityNumber: string;
  taxDepartment: string;
  phoneNumber: string;
  email: string;
  serverName: string;
  databaseName: string;
  serverUserId: string;
  serverPassword: string;
}

export type UpdateCompanyRequest = CreateCompanyRequest;

export interface Role {
  id: string;
  name: string;
  code: string;
  status: boolean;
  createDate: string;
  editDate: string | null;
}

export interface CreateRoleRequest {
  name: string;
  code: string;
}

export interface UpdateRoleRequest {
  name: string;
  code: string;
  status: boolean;
}

export interface MainRole {
  id: string;
  title: string;
  isRoleCreateByAdmin: boolean;
  companyId: string;
}

export interface CreateMainRoleRequest {
  title: string;
  isRoleCreateByAdmin: boolean;
  companyId: string;
}

export type UpdateMainRoleRequest = CreateMainRoleRequest;

export interface MainRoleAndRoleRelationship {
  id: string;
  roleId: string;
  mainRoleId: string;
}

export interface CreateMainRoleAndRoleRelationshipRequest {
  roleId: string;
  mainRoleId: string;
}

export type UpdateMainRoleAndRoleRelationshipRequest = CreateMainRoleAndRoleRelationshipRequest;

export interface MainRoleAndUserRelationship {
  id: string;
  userId: string;
  mainRoleId: string;
  companyId: string;
}

export interface CreateMainRoleAndUserRelationshipRequest {
  userId: string;
  mainRoleId: string;
  companyId: string;
}

export type UpdateMainRoleAndUserRelationshipRequest = CreateMainRoleAndUserRelationshipRequest;

export interface UserListItem {
  id: string;
  userName: string | null;
  email: string | null;
  status: boolean;
  createDate: string;
  editDate: string | null;
}

export interface MyProfile {
  id: string;
  firstName: string | null;
  lastName: string | null;
  userName: string | null;
  email: string | null;
  phoneNumber: string | null;
  isAdmin: boolean;
}

export interface UpdateMyProfileRequest {
  userName: string;
  email: string;
  phoneNumber?: string;
  firstName?: string;
  lastName?: string;
}

export interface ChangePasswordRequest {
  currentPassword: string;
  newPassword: string;
  confirmNewPassword: string;
}

export interface UniformChartOfAccount {
  id: string;
  code: string;
  name: string;
  type: string;
}

export interface CreateUniformChartOfAccountRequest {
  code: string;
  name: string;
  type: string;
}

export type UpdateUniformChartOfAccountRequest = CreateUniformChartOfAccountRequest;

export interface SeedSampleDataResult {
  permissionRolesCreated: number;
  companiesCreated: number;
  usersCreated: number;
  userCompanyLinksCreated: number;
  mainRolesCreated: number;
  mainRoleRoleLinksCreated: number;
  mainRoleRoleLinksRemoved: number;
  mainRoleUserLinksCreated: number;
}
