import type { ReactNode } from "react";
import { Navigate, useLocation } from "react-router-dom";
import { useAppSelector } from "../app/hooks/hooks";

interface RoleProtectedRouteProps {
    children: ReactNode,
    allowedRoles: string[]
}

function RoleProtectedRoute({ children, allowedRoles }: RoleProtectedRouteProps) {
    const { isAuthenticated, user } = useAppSelector(state => state.auth);
    const location = useLocation();

    if (!isAuthenticated) {
        return <Navigate to='/login' state={{ from: location }} replace />
    }

    if (!user || !allowedRoles.includes(user.role)) {
        return <Navigate to='/unauthorized' replace />
    }

    return (<>{children}</>);
}

export default RoleProtectedRoute;