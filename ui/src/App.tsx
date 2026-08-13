import { BrowserRouter as Router, Routes, Route } from "react-router";
import SignIn from "./pages/AuthPages/SignIn";
import SignUp from "./pages/AuthPages/SignUp";
import NotFound from "./pages/OtherPage/NotFound";
import UserProfiles from "./pages/UserProfiles";
import Videos from "./pages/UiElements/Videos";
import Images from "./pages/UiElements/Images";
import Alerts from "./pages/UiElements/Alerts";
import Badges from "./pages/UiElements/Badges";
import Avatars from "./pages/UiElements/Avatars";
import Buttons from "./pages/UiElements/Buttons";
import LineChart from "./pages/Charts/LineChart";
import BarChart from "./pages/Charts/BarChart";
import Calendar from "./pages/Calendar";
import BasicTables from "./pages/Tables/BasicTables";
import FormElements from "./pages/Forms/FormElements";
import Blank from "./pages/Blank";
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
            <Route path="/calendar" element={<Calendar />} />
            <Route path="/blank" element={<Blank />} />

            {/* Forms */}
            <Route path="/form-elements" element={<FormElements />} />

            {/* Tables */}
            <Route path="/basic-tables" element={<BasicTables />} />

            {/* Ui Elements */}
            <Route path="/alerts" element={<Alerts />} />
            <Route path="/avatars" element={<Avatars />} />
            <Route path="/badge" element={<Badges />} />
            <Route path="/buttons" element={<Buttons />} />
            <Route path="/images" element={<Images />} />
            <Route path="/videos" element={<Videos />} />

            {/* Charts */}
            <Route path="/line-chart" element={<LineChart />} />
            <Route path="/bar-chart" element={<BarChart />} />

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
