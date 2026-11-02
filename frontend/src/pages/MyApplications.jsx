import { useEffect, useState } from "react";
import { getMyApplications } from "../api/applications";
import { useNavigate } from "react-router-dom";

export default function MyApplications() {
    const navigate = useNavigate();
    const [applications, setApplications] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    useEffect(() => {
        const load = async () => {
            setLoading(true);
            setError("");
            try {
                const data = await getMyApplications();
                setApplications(Array.isArray(data) ? data : []);
            } catch (err) {
                setError(err.message);
            } finally {
                setLoading(false);
            }
        };
        void load();
    }, []);

    return (
        <div className="layout-main">
            <div className="page-header">
                <div>
                    <h1>My Applications</h1>
                    <p>Track the status of your event applications.</p>
                </div>
                <button className="back-btn" onClick={() => navigate("/dashboard")}>
                    ← Back to Dashboard
                </button>
            </div>

            {error && <p className="msg msg-error" role="alert">{error}</p>}
            {loading && <p>Loading your applications...</p>}
            {!loading && applications.length === 0 && (
                <div className="empty-state"><p>No applications yet.</p></div>
            )}

            <div style={{ display: "grid", gap: 16 }}>
                {applications.map((a) => (
                    <article key={a.id} className="card">
                        <h3>{a.eventTitle}</h3>
                        <p><strong>Departments:</strong> {(a.eventDepartments || []).join(", ")}</p>
                        <p><strong>Email:</strong> {a.email}</p>
                        <p><strong>Phone:</strong> {a.phoneNumber}</p>
                        <p>
                            <strong>Status:</strong>{" "}
                            <span style={{ color: a.status === "Approved" ? "#16a34a" : a.status === "Rejected" ? "#dc2626" : "#ca8a04" }}>
                                {a.status}
                            </span>
                        </p>
                        {a.reviewerNotes && <p><strong>Notes:</strong> {a.reviewerNotes}</p>}
                    </article>
                ))}
            </div>
        </div>
    );
}
