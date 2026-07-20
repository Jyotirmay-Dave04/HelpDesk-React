import { Link } from "react-router-dom";

function UnAuthorizedPage() {
    return ( 
        <div>
            <h1>You are UNAUTHORIZED to page</h1>
            <br />
            <Link to='/dashboard'>Go to Dashboard</Link>
        </div>
     );
}

export default UnAuthorizedPage;