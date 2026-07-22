import { Link } from "react-router-dom";
import { useAppSelector } from "../app/hooks/hooks";
import { UserRole } from "../types/user-role";

function UnAuthorizedPage() {
    const { user } = useAppSelector(state => state.auth);

    return (
        <div>
            <h1>You are UNAUTHORIZED to page</h1>
            <br />
            <Link to={user.role === UserRole.Requester ? '/myTickets' : '/dashboard'}>Go to Home</Link>
        </div>
    );
}

export default UnAuthorizedPage;