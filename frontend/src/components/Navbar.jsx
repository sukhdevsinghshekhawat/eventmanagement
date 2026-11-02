import { NavLink, useNavigate } from "react-router-dom";
import { getUserRole, getUserFullName, getUserDepartment, logoutUser } from "../api/auth";

export default function Navbar() {
    const navigate = useNavigate();
    const role = getUserRole();
    const fullName = getUserFullName();
    const department = getUserDepartment();

    const handleLogout = () => {
        logoutUser();
        navigate("/login");
    };

    const navItems = {
        Admin: [
            { label: "Dashboard", to: "/dashboard" },
            { label: "Events", to: "/events" },
            { label: "Applications", to: "/applications" },
            { label: "Manage Teachers", to: "/admin/teachers" },
        ],
        Teacher: [
            { label: "Dashboard", to: "/dashboard" },
            { label: "My Events", to: "/events" },
            { label: "Student Applications", to: "/applications" },
        ],
        Student: [
            { label: "Dashboard", to: "/dashboard" },
            { label: "College Events", to: "/events" },
            { label: "My Applications", to: "/my-applications" },
        ],
    }[role] || [];

    return (
        <nav className="navbar">
            <div className="navbar-inner">
                <div className="navbar-brand">
                    <NavLink to="/" end className="brand-link">
                        <span className="brand-icon">🎓</span>
                        <span className="brand-name">College<span className="brand-accent">Event</span></span>
                    </NavLink>
                </div>

                <div className="navbar-nav">
                    {navItems.map((item) => (
                        <NavLink
                            key={item.to}
                            to={item.to}
                            end={item.to === "/dashboard"}
                            className={({ isActive }) => `nav-link ${isActive ? "active" : ""}`}
                        >
                            {item.label}
                        </NavLink>
                    ))}
                </div>

                <div className="navbar-user">
                    {role && (
                        <div className="user-info">
                            <div className="user-name">{fullName}</div>
                            <div className="user-meta">
                                {department} • {role}
                            </div>
                        </div>
                    )}
                    <button className="btn-logout" onClick={handleLogout}>Logout</button>
                </div>
            </div>
        </nav>
    );
}
