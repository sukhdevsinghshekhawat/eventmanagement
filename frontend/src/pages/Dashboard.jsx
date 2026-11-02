import { Link, useNavigate } from "react-router-dom";
import { getUserRole, logoutUser } from "../api/auth";

export default function Dashboard() {
    const navigate = useNavigate();
    const role = getUserRole();

    function handleLogout() {
        logoutUser();
        navigate("/login");
    }

    const dashboardItems = {
        Admin: [
            { title: "Manage Events", description: "View and create college events.", link: "/events" },
            { title: "Manage Teachers", description: "Manage teacher accounts.", link: "/admin/teachers" },
            { title: "Applications", description: "Review student event applications.", link: "/applications" },
        ],
        Teacher: [
            { title: "My Events", description: "Create and manage your events.", link: "/events" },
            { title: "Student Applications", description: "Review applications for your events.", link: "/applications" },
        ],
        Student: [
            { title: "College Events", description: "Explore available events.", link: "/events" },
            { title: "My Applications", description: "Check your application status.", link: "/my-applications" },
        ],
    };

    const items = dashboardItems[role] || [];

    return (
        <div className="layout-main">
            <div className="page-header">
                <div>
                    <h1>Dashboard</h1>
                    <p>Welcome back, {role ? role : "User"}!</p>
                </div>
                <button className="btn-outline" onClick={handleLogout}>Logout</button>
            </div>

            {!role && <p className="msg msg-error">Your role could not be read. Please log in again.</p>}

            <section className="dashboard-grid">
                {items.map((item) => (
                    <Link key={item.link} to={item.link} className="dashboard-card-link">
                        <div className="card dashboard-card">
                            <h3>{item.title}</h3>
                            <p>{item.description}</p>
                        </div>
                    </Link>
                ))}
            </section>
        </div>
    );
}
