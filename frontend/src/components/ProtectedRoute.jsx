import { Navigate } from "react-router-dom";
import { getToken, getUserRole } from "../api/auth";

export default function ProtectedRoute({ children, roles }) {
    const token = getToken();

    if (!token) {
        return <Navigate to="/login" replace />;
    }

    if (roles && roles.length > 0) {
        const role = getUserRole();
        if (!role || !roles.includes(role)) {
            return <Navigate to="/dashboard" replace />;
        }
    }

    return children;
}
