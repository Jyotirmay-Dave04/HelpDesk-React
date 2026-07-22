import type { ReactNode } from "react";
import { Navigate } from "react-router-dom";
import { useAppSelector } from "../app/hooks/hooks";
import type { UserRole } from "../types/user-role";

interface RoleProtectedRouteProps {
    children: ReactNode,
    allowedRoles: UserRole[]
}

function RoleProtectedRoute({ children, allowedRoles }: RoleProtectedRouteProps) {
    const { isAuthenticated, user } = useAppSelector(state => state.auth);

    if (!isAuthenticated) {
        return <Navigate to='/login' replace />
    }

    if (!user || !allowedRoles.includes(user.role as UserRole)) {
        return <Navigate to='/unauthorized' replace />
    }

    return (<>{children}</>);
}

export default RoleProtectedRoute;