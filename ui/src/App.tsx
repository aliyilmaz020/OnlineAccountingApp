import { BrowserRouter as Router, Routes, Route } from "react-router";
import SignIn from "./pages/AuthPages/SignIn";
import SignUp from "./pages/AuthPages/SignUp";
import NotFound from "./pages/OtherPage/NotFound";
import UserProfiles from "./pages/UserProfiles";
import AppLayout from "./layout/AppLayout";
import { ScrollToTop } from "./components/common/ScrollToTop";
import Home from "./pages/Dashboard/Home";
import ProtectedRoute from "./routes/ProtectedRoute";
import RequireCompany from "./routes/RequireCompany";
import SelectCompany from "./pages/Company/SelectCompany";
import CompaniesListPage from "./pages/Companies/CompaniesListPage";
import RolesListPage from "./pages/Roles/RolesListPage";
import MainRolesListPage from "./pages/MainRoles/MainRolesListPage";
import MainRoleAndRoleRelationshipsListPage from "./pages/MainRoleAndRoleRelationships/MainRoleAndRoleRelationshipsListPage";
import MainRoleAndUserRelationshipsListPage from "./pages/MainRoleAndUserRelationships/MainRoleAndUserRelationshipsListPage";
import UniformChartOfAccountsListPage from "./pages/UniformChartOfAccounts/UniformChartOfAccountsListPage";

export default function App() {
  return (
    <>
      <Router>
        <ScrollToTop />
        <Routes>
          {/* Dashboard Layout */}
          <Route
            element={
              <ProtectedRoute>
                <AppLayout />
              </ProtectedRoute>
            }
          >
            <Route index path="/" element={<Home />} />

            {/* Others Page */}
            <Route path="/profile" element={<UserProfiles />} />

            {/* Accounting */}
            <Route path="/companies" element={<CompaniesListPage />} />
            <Route path="/roles" element={<RolesListPage />} />
            <Route path="/main-roles" element={<MainRolesListPage />} />
            <Route
              path="/main-role-role-relationships"
              element={<MainRoleAndRoleRelationshipsListPage />}
            />
            <Route
              path="/main-role-user-relationships"
              element={<MainRoleAndUserRelationshipsListPage />}
            />
            <Route
              path="/uniform-chart-of-accounts"
              element={
                <RequireCompany>
                  <UniformChartOfAccountsListPage />
                </RequireCompany>
              }
            />
          </Route>

          {/* Company selection (authenticated, outside AppLayout) */}
          <Route
            path="/select-company"
            element={
              <ProtectedRoute>
                <SelectCompany />
              </ProtectedRoute>
            }
          />

          {/* Auth Layout */}
          <Route path="/signin" element={<SignIn />} />
          <Route path="/signup" element={<SignUp />} />

          {/* Fallback Route */}
          <Route path="*" element={<NotFound />} />
        </Routes>
      </Router>
    </>
  );
}
