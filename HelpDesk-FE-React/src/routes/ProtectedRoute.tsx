import { Navigate, useLocation } from "react-router-dom";
import { useAppSelector } from "../app/hooks/hooks";

function ProtectedRoute({ children }) {
    const { isAuthenticated } = useAppSelector(state => state.auth);
    const location = useLocation();

    if (!isAuthenticated) {
        return <Navigate to='/login' state={{ from: location }} replace></Navigate>
    }

    return <>{children}</>
}

export default ProtectedRoute;