import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { getUserDepartment, getUserRole } from "../api/auth";
import { submitApplication } from "../api/applications";
import { createEvent, deleteEvent, getEvents } from "../api/events";
import { getTeachers } from "../api/admin";
import { validateRequiredText } from "../utils/validation";

const DEPARTMENTS = ["MCA", "BCA", "B.Tech", "MBA", "BBA"];
const emptyForm = { title: "", description: "", departments: [], venue: "", eventDate: "" };

export default function Events() {
  const navigate = useNavigate();
  const role = getUserRole();
  const department = getUserDepartment();
  const canManage = role === "Admin" || role === "Teacher";
  const [events, setEvents] = useState([]);
  const [teachers, setTeachers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");
  const [message, setMessage] = useState("");
  const [form, setForm] = useState(emptyForm);
  const [applyState, setApplyState] = useState(null);

  async function loadEvents() {
    setLoading(true);
    setError("");
    try {
      const data = await getEvents();
      setEvents(Array.isArray(data) ? data : []);
      if (role === "Admin") {
        const teacherData = await getTeachers();
        setTeachers(Array.isArray(teacherData) ? teacherData : []);
      }
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    let cancelled = false;
    async function loadInitialEvents() {
      try {
        const data = await getEvents();
        const teacherData = role === "Admin" ? await getTeachers() : [];
        if (cancelled) return;
        setEvents(Array.isArray(data) ? data : []);
        setTeachers(Array.isArray(teacherData) ? teacherData : []);
      } catch (err) {
        if (!cancelled) setError(err.message);
      } finally {
        if (!cancelled) setLoading(false);
      }
    }
    void loadInitialEvents();
    return () => { cancelled = true; };
  }, [role]);

  function toggleDepartment(value) {
    setForm((current) => ({
      ...current,
      departments: current.departments.includes(value)
        ? current.departments.filter((item) => item !== value)
        : [...current.departments, value],
    }));
  }

  async function handleCreate(event) {
    event.preventDefault();
    setError("");
    setMessage("");
    const requiredFieldErrors = [
      validateRequiredText(form.title, "Event title"),
      validateRequiredText(form.description, "Description"),
      validateRequiredText(form.venue, "Venue"),
    ].filter(Boolean);
    if (requiredFieldErrors.length) return setError(requiredFieldErrors.join(" "));
    if (!form.departments.length) return setError("Select at least one department.");
    setSaving(true);
    try {
      await createEvent({ ...form, title: form.title.trim(), description: form.description.trim(), venue: form.venue.trim(), eventDate: new Date(form.eventDate).toISOString() });
      setMessage("Event created successfully.");
      setForm(emptyForm);
      await loadEvents();
    } catch (err) {
      setError(err.message);
    } finally {
      setSaving(false);
    }
  }

  async function handleDelete(eventId) {
    if (!window.confirm("Are you sure you want to delete this event?")) return;
    setError("");
    setMessage("");
    try {
      await deleteEvent(eventId);
      setEvents((current) => current.filter((item) => item.id !== eventId));
      setMessage("Event deleted successfully.");
    } catch (err) {
      setError(err.message);
    }
  }

  async function handleApply(event) {
    event.preventDefault();
    setError("");
    setMessage("");
    try {
      await submitApplication({ eventId: applyState.eventId, email: applyState.email, phoneNumber: applyState.phoneNumber });
      setApplyState(null);
      setMessage("Your application was submitted. You can track its status in My Applications.");
    } catch (err) {
      setError(err.message);
    }
  }

  return (
    <div className="events-page">
      <div className="page-header">
        <div><h1>{role === "Student" ? "College Events" : role === "Teacher" ? "Department Events" : "All College Events"}</h1>
          <p>{role === "Student" ? `Events available for ${department || "your department"}` : "Manage events and applications in one place."}</p></div>
        {role === "Student" && <button className="btn-outline" onClick={() => navigate("/my-applications")}>My Applications</button>}
      </div>

      {error && <p className="msg msg-error" role="alert">{error}</p>}
      {message && <p className="msg msg-success" role="status">{message}</p>}

      {canManage && <section className="form-section">
        <h2>Create an Event</h2>
        <form onSubmit={handleCreate}>
          <div className="form-group"><label htmlFor="event-title">Event title</label><input id="event-title" value={form.title} onChange={(e) => setForm({ ...form, title: e.target.value })} maxLength={150} required /></div>
          <div className="form-group"><label htmlFor="event-description">Description</label><textarea id="event-description" value={form.description} onChange={(e) => setForm({ ...form, description: e.target.value })} maxLength={2000} required /></div>
          <div className="form-group"><span className="form-label">Which departments can participate?</span>
            <div className="department-checkboxes">{DEPARTMENTS.map((item) => <label className="dept-checkbox" key={item}><input type="checkbox" checked={form.departments.includes(item)} onChange={() => toggleDepartment(item)} />{item}</label>)}</div>
          </div>
          <div className="form-row"><div className="form-group"><label htmlFor="event-venue">Venue</label><input id="event-venue" value={form.venue} onChange={(e) => setForm({ ...form, venue: e.target.value })} maxLength={100} required /></div>
            <div className="form-group"><label htmlFor="event-date">Date and time</label><input id="event-date" type="datetime-local" value={form.eventDate} onChange={(e) => setForm({ ...form, eventDate: e.target.value })} required /></div></div>
          <button className="btn-primary" type="submit" disabled={saving}>{saving ? "Creating…" : "Create Event"}</button>
        </form>
      </section>}

      <section>
        <div className="list-header"><h2>Available Events</h2><span className="muted">{events.length} events</span></div>
        {loading && <p className="muted">Loading events…</p>}
        {!loading && events.length === 0 && <div className="empty-state"><p>No events are available yet.</p></div>}
        <div className="event-grid">{events.map((item) => <article className="card event-card" key={item.id}>
          <div className="event-card-top"><span className="event-tag">{(item.departments || []).join(" · ")}</span><span className="event-date">{new Date(item.eventDate).toLocaleString()}</span></div>
          <h3>{item.title}</h3><p>{item.description}</p>
          <div className="event-meta"><span>📍 {item.venue}</span><span>👥 {item.departments?.length || 0} department</span>
            {role === "Admin" && <span>Teacher: {teachers.find((teacher) => teacher.id === item.createdByUserId)?.fullName || teachers.find((teacher) => teacher.id === item.createdByUserId)?.email || "Admin"}</span>}</div>
          <div className="event-actions">{role === "Student" && <button className="btn-primary" onClick={() => { setError(""); setApplyState({ eventId: item.id, email: "", phoneNumber: "" }); }}>Apply to Participate</button>}
            {role === "Admin" && <button className="btn-danger" onClick={() => handleDelete(item.id)}>Delete Event</button>}</div>
        </article>)}</div>
      </section>

      {applyState && <div className="modal-backdrop" role="presentation" onMouseDown={(e) => { if (e.target === e.currentTarget) setApplyState(null); }}>
        <form className="apply-modal" onSubmit={handleApply} role="dialog" aria-modal="true" aria-labelledby="apply-title">
          <button className="modal-close" type="button" aria-label="Close" onClick={() => setApplyState(null)}>×</button>
          <h2 id="apply-title">Apply to Participate</h2><p>Enter your contact details. A teacher will review your application.</p>
          <div className="form-group"><label htmlFor="apply-email">Email</label><input id="apply-email" type="email" maxLength={256} value={applyState.email} onChange={(e) => setApplyState({ ...applyState, email: e.target.value })} required /></div>
          <div className="form-group"><label htmlFor="apply-phone">Phone number</label><input id="apply-phone" type="tel" inputMode="tel" pattern="[0-9+() -]{7,20}" title="Enter 7–20 characters using digits, spaces, +, parentheses, or hyphens." maxLength={30} value={applyState.phoneNumber} onChange={(e) => setApplyState({ ...applyState, phoneNumber: e.target.value })} required /></div>
          <div className="event-actions"><button type="submit" className="btn-primary">Submit Application</button><button type="button" className="btn-outline" onClick={() => setApplyState(null)}>Cancel</button></div>
        </form>
      </div>}
    </div>
  );
}
