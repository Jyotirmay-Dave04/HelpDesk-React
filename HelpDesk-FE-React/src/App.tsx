import { Routes, Route, Outlet } from "react-router-dom";
import LoginForm from "./pages/LoginPage";
import ProtectedRoute from "./routes/ProtectedRoute";
import { ToastConnector } from "./utils/Toast";
import UnAuthorizedPage from "./pages/UnauthorizedPage";
import RoleProtectedRoute from "./routes/RoleProtectedRoute";
import { UserRole } from "./types/user-role";
import LoggedRoute from "./routes/LoggedRoute";
import CreateTicketPage from "./pages/TicketCreatePage";
import TicketListPage from "./pages/TicketListPage";
import TicketDetailsPage from "./pages/TicketDetailsPage";
import UserManagementPage from "./pages/UserManagementPage";
import AppLayout from "./layouts/AppLayout";
import GroupManagementPage from "./pages/GroupManagementPage";
import SlaSettingsPage from "./pages/SlaSettingsPage";

function App() {
  return (
    <>
      <ToastConnector />
      <Routes>
        <Route path="/unauthorized" element={<UnAuthorizedPage />}></Route>
        <Route path='/login' element={
          <LoggedRoute><LoginForm /></LoggedRoute>
        }></Route>

        <Route element={<ProtectedRoute />}>
          <Route element={<AppLayout />}>
            <Route path='/dashboard' element={<TicketListPage />} />
            <Route path='/myTickets' element={<TicketListPage />} />
            <Route path='/allTickets' element={<TicketListPage />} />
            <Route path='/ticket/create' element={<CreateTicketPage />} />
            <Route path='/ticket/edit/:id' element={<CreateTicketPage />} />
            <Route path='/ticket/:ticketId' element={<TicketDetailsPage />} />

            <Route element={<RoleProtectedRoute allowedRoles={[UserRole.Admin]}><Outlet /></RoleProtectedRoute>}>
              <Route path='/userManagement' element={<UserManagementPage />} />
              <Route path='/groupManagement' element={<GroupManagementPage />} />
              <Route path='/slaSettings' element={<SlaSettingsPage />} />
            </Route>
          </Route>
        </Route>

      </Routes>
    </>
  );
}

export default App;