import { Navigate, Outlet } from "react-router-dom";
import { useAppSelector } from "../app/hooks/hooks";

function ProtectedRoute() {
    const { isAuthenticated } = useAppSelector(state => state.auth);

    if (!isAuthenticated) {
        return <Navigate to='/login' replace></Navigate>
    }

    return <Outlet />;
}

export default ProtectedRoute;