import { Navigate } from "react-router-dom";
import { useAppSelector } from "../app/hooks/hooks";

function LoggedRoute({ children }) {
    const { isAuthenticated } = useAppSelector(state => state.auth);

    if (isAuthenticated) {
        return <Navigate replace to='/dashboard' />
    }

    return <>{children}</>
}

export default LoggedRoute;