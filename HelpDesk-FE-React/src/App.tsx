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

function App() {
  return (
    <>
      <ToastConnector />
      <Routes>
        <Route path="/unauthorized" element={<UnAuthorizedPage />}></Route>
        <Route path='/login' element={
          <LoggedRoute><LoginForm /></LoggedRoute>
        }></Route>

        <Route element={<ProtectedRoute><Outlet /></ProtectedRoute>}>
          <Route path='/dashboard' element={<TicketListPage />} />
          <Route path='/ticket/create' element={<CreateTicketPage />} />
          <Route path='/ticket/edit/:id' element={<CreateTicketPage />} />
          <Route path='/ticket/:ticketId' element={<TicketDetailsPage />} />
        </Route>

        <Route element={<RoleProtectedRoute allowedRoles={[UserRole.Admin]}><Outlet /></RoleProtectedRoute>}>
        </Route>
      </Routes>
    </>
  );
}

export default App;