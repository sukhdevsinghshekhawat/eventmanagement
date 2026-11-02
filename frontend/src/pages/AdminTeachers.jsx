import { useEffect, useState } from "react";
import { createTeacher, getTeachers, deleteTeacher, getStudents, setUserStatus } from "../api/admin";
import { useNavigate } from "react-router-dom";
import { validatePassword, validateRequiredText } from "../utils/validation";

const DEPARTMENTS = ["MCA", "BCA", "B.Tech", "MBA", "BBA"];

export default function AdminTeachers() {
    const navigate = useNavigate();
    const [teachers, setTeachers] = useState([]);
    const [students, setStudents] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");
    const [message, setMessage] = useState("");
    const [saving, setSaving] = useState(false);
    const [form, setForm] = useState({
        fullName: "",
        email: "",
        password: "",
        department: "MCA",
    });

    const load = async () => {
        setLoading(true);
        setError("");
        try {
            const data = await getTeachers();
            setTeachers(Array.isArray(data) ? data : []);
            const studentData = await getStudents();
            setStudents(Array.isArray(studentData) ? studentData : []);
        } catch (err) {
            setError(err.message);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        void load();
    }, []);

    function handleChange(e) {
        setForm({ ...form, [e.target.name]: e.target.value });
    }

    async function handleSubmit(e) {
        e.preventDefault();
        setError("");
        setMessage("");
        const nameError = validateRequiredText(form.fullName, "Full name");
        if (nameError) return setError(nameError);
        const passwordError = validatePassword(form.password);
        if (passwordError) return setError(passwordError);
        setSaving(true);
        try {
            await createTeacher({ ...form, fullName: form.fullName.trim(), email: form.email.trim() });
            setMessage("Teacher created successfully.");
            setForm({ fullName: "", email: "", password: "", department: "MCA" });
            await load();
        } catch (err) {
            setError(err.message);
        } finally {
            setSaving(false);
        }
    }

    async function handleDelete(id) {
        if (!window.confirm("Delete this teacher?")) return;
        setError("");
        setMessage("");
        try {
            await deleteTeacher(id);
            setMessage("Teacher deleted.");
            await load();
        } catch (err) {
            setError(err.message);
        }
    }

    async function handleStudentStatus(student) {
        setError("");
        try {
            await setUserStatus(student.id, !student.isActive);
            setStudents((current) => current.map((item) => item.id === student.id ? { ...item, isActive: !item.isActive } : item));
            setMessage(`Student account ${student.isActive ? "disabled" : "enabled"}.`);
        } catch (err) {
            setError(err.message);
        }
    }

    return (
        <div className="layout-main">
            <div className="page-header">
                <div>
                    <h1>Manage Teachers</h1>
                    <p>Create teacher accounts. Only admins can do this.</p>
                </div>
                <button className="back-btn" onClick={() => navigate("/dashboard")}>
                    ← Back to Dashboard
                </button>
            </div>

            {error && <p className="msg msg-error" role="alert">{error}</p>}
            {message && <p className="msg msg-success" role="status">{message}</p>}

            <div className="form-section">
                <h2>Create Teacher</h2>
                <form onSubmit={handleSubmit}>
                    <div className="form-group">
                        <label htmlFor="teacher-name">Full Name</label>
                        <input id="teacher-name" name="fullName" placeholder="Full name" value={form.fullName} onChange={handleChange} maxLength={150} autoComplete="name" required />
                    </div>
                    <div className="form-group">
                        <label htmlFor="teacher-email">Email</label>
                        <input id="teacher-email" name="email" type="email" placeholder="Email" value={form.email} onChange={handleChange} maxLength={256} autoComplete="email" required />
                    </div>
                    <div className="form-group">
                        <label htmlFor="teacher-password">Password</label>
                        <input id="teacher-password" name="password" type="password" placeholder="Create a password" value={form.password} onChange={handleChange} required minLength={8} maxLength={128} autoComplete="new-password" />
                        <small className="field-hint">Use at least 8 characters, including uppercase and lowercase letters, a number, and a special character.</small>
                    </div>
                    <div className="form-group">
                        <label htmlFor="teacher-department">Department</label>
                        <select id="teacher-department" name="department" value={form.department} onChange={handleChange} required>
                            {DEPARTMENTS.map((d) => <option key={d} value={d}>{d}</option>)}
                        </select>
                    </div>
                    <button type="submit" className="btn-primary" disabled={saving}>{saving ? "Creating…" : "Create Teacher"}</button>
                </form>
            </div>

            <section>
                <h2 style={{ fontSize: "1.25rem", marginBottom: 16 }}>Existing Teachers</h2>
                {loading && <p>Loading...</p>}
                {!loading && teachers.length === 0 && <p>No teachers yet.</p>}
                <div style={{ display: "grid", gap: 12 }}>
                    {teachers.map((t) => (
                        <article key={t.id} className="card" style={{ display: "flex", justifyContent: "space-between", alignItems: "center", padding: 16 }}>
                            <div>
                                <div style={{ fontWeight: 600, color: "#0f172a" }}>{t.fullName || t.email}</div>
                                <div style={{ color: "#64748b", fontSize: 0.9 }}>{t.email}</div>
                                <div style={{ color: "#64748b", fontSize: 0.9 }}>Department: {t.department}</div>
                            </div>
                            <button className="btn-danger" onClick={() => handleDelete(t.id)}>Delete</button>
                        </article>
                    ))}
                </div>
            </section>

            <section style={{ marginTop: 32 }}>
                <h2 style={{ fontSize: "1.25rem", marginBottom: 16 }}>Student accounts</h2>
                {loading && <p>Loading...</p>}
                {!loading && students.length === 0 && <p>No students yet.</p>}
                <div style={{ display: "grid", gap: 12 }}>
                    {students.map((student) => (
                        <article key={student.id} className="card admin-user-row">
                            <div>
                                <div style={{ fontWeight: 600 }}>{student.fullName || student.email}</div>
                                <div className="muted">{student.email} · {student.department}</div>
                                <span className={`status-pill ${student.isActive ? "status-approved" : "status-rejected"}`}>{student.isActive ? "Active" : "Disabled"}</span>
                            </div>
                            <button className={student.isActive ? "btn-danger" : "btn-success"} onClick={() => handleStudentStatus(student)}>{student.isActive ? "Disable account" : "Enable account"}</button>
                        </article>
                    ))}
                </div>
            </section>
        </div>
    );
}
