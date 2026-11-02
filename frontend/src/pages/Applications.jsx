import { useEffect, useState } from "react";
import { getAllApplications, reviewApplication } from "../api/applications";
import { useNavigate } from "react-router-dom";

export default function Applications() {
    const navigate = useNavigate();
    const [applications, setApplications] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");
    const [message, setMessage] = useState("");
    const [notes, setNotes] = useState({});

    const load = async () => {
        setLoading(true);
        setError("");
        try {
            const data = await getAllApplications();
            setApplications(Array.isArray(data) ? data : []);
        } catch (err) {
            setError(err.message);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        let cancelled = false;
        async function loadInitialApplications() {
            try {
                const data = await getAllApplications();
                if (!cancelled) setApplications(Array.isArray(data) ? data : []);
            } catch (err) {
                if (!cancelled) setError(err.message);
            } finally {
                if (!cancelled) setLoading(false);
            }
        }
        void loadInitialApplications();
        return () => { cancelled = true; };
    }, []);

    const statusToEnum = { Approved: 1, Rejected: 2 };

    async function handleReview(id, status) {
        setError("");
        setMessage("");
        try {
            await reviewApplication(id, {
                status: statusToEnum[status] ?? 0,
                notes: notes[id] || "",
            });
            setMessage(`Application ${status.toLowerCase()}.`);
            await load();
        } catch (err) {
            setError(err.message);
        }
    }

    return (
        <div className="layout-main">
            <div className="page-header">
                <div>
                    <h1>Student Applications</h1>
                    <p>Review applications submitted for your department events.</p>
                </div>
                <button className="back-btn" onClick={() => navigate("/dashboard")}>
                    ← Back to Dashboard
                </button>
            </div>

            {error && <p className="msg msg-error" role="alert">{error}</p>}
            {message && <p className="msg msg-success" role="status">{message}</p>}
            {loading && <p>Loading applications...</p>}
            {!loading && applications.length === 0 && (
                <div className="empty-state"><p>No applications found.</p></div>
            )}

            <div style={{ display: "grid", gap: 16 }}>
                {applications.map((a) => (
                    <article key={a.id} className="card">
                        <h3>{a.eventTitle}</h3>
                        <p><strong>Student:</strong> {a.applicantName} ({a.email})</p>
                        <p><strong>Phone:</strong> {a.phoneNumber}</p>
                        <p><strong>Department:</strong> {a.department}</p>
                        <p><strong>Event Departments:</strong> {(a.eventDepartments || []).join(", ")}</p>
                        <p>
                            <strong>Status:</strong>{" "}
                            <span style={{ color: a.status === "Approved" ? "#16a34a" : a.status === "Rejected" ? "#dc2626" : "#ca8a04" }}>
                                {a.status}
                            </span>
                        </p>
                        {a.status === "Pending" && (
                            <div style={{ marginTop: 12 }}>
                                <textarea
                                    placeholder="Reviewer notes (optional)"
                                    aria-label={`Reviewer notes for ${a.applicantName}`}
                                    maxLength={500}
                                    value={notes[a.id] || ""}
                                    onChange={(e) => setNotes({ ...notes, [a.id]: e.target.value })}
                                    style={{ width: "100%", minHeight: 60, padding: 8, boxSizing: "border-box" }}
                                />
                                <div className="event-actions">
                                    <button className="btn-success" onClick={() => handleReview(a.id, "Approved")}>Approve</button>
                                    <button className="btn-danger" onClick={() => handleReview(a.id, "Rejected")}>Reject</button>
                                </div>
                            </div>
                        )}
                        {a.reviewerNotes && <p style={{ marginTop: 8 }}><strong>Notes:</strong> {a.reviewerNotes}</p>}
                    </article>
                ))}
            </div>
        </div>
    );
}
