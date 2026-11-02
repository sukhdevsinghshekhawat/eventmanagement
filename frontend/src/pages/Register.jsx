import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { registerUser } from "../api/auth";
import { validatePassword, validateRequiredText } from "../utils/validation";

const DEPARTMENTS = ["MCA", "BCA", "B.Tech", "MBA", "BBA"];

export default function Register() {
    const navigate = useNavigate();

    const [formData, setFormData] = useState({
        fullName: "",
        email: "",
        password: "",
        confirmPassword: "",
        department: "MCA",
    });

    const [error, setError] = useState("");
    const [success, setSuccess] = useState("");
    const [loading, setLoading] = useState(false);

    function handleChange(e) {
        setFormData({
            ...formData,
            [e.target.name]: e.target.value,
        });
    }

    async function handleSubmit(e) {
        e.preventDefault();
        setError("");
        setSuccess("");

        const nameError = validateRequiredText(formData.fullName, "Full name");
        if (nameError) {
            setError(nameError);
            return;
        }

        if (formData.password !== formData.confirmPassword) {
            setError("Passwords do not match.");
            return;
        }

        const passwordError = validatePassword(formData.password);
        if (passwordError) {
            setError(passwordError);
            return;
        }

        setLoading(true);
        try {
            const result = await registerUser(formData);
            setSuccess(result.message || "Registration successful!");

            setTimeout(() => {
                navigate("/login");
            }, 1200);
        } catch (err) {
            setError(err.message || "Registration failed.");
        } finally {
            setLoading(false);
        }
    }

    return (
        <div className="auth-container">
            <form className="auth-card" onSubmit={handleSubmit}>
                <h1>Create Account</h1>
                <p className="auth-subtitle">
                    Register as a student to access college events.
                </p>

                {error && <div className="error-message" role="alert">{error}</div>}
                {success && <div className="success-message" role="status">{success}</div>}

                <label htmlFor="fullName">Full Name</label>
                <input
                    id="fullName"
                    type="text"
                    name="fullName"
                    placeholder="Enter your full name"
                    value={formData.fullName}
                    onChange={handleChange}
                    maxLength={150}
                    autoComplete="name"
                    required
                />

                <label htmlFor="email">Email Address</label>
                <input
                    id="email"
                    type="email"
                    name="email"
                    placeholder="Enter your email"
                    value={formData.email}
                    onChange={handleChange}
                    maxLength={256}
                    autoComplete="email"
                    required
                />

                <label htmlFor="department">Department</label>
                <select
                    id="department"
                    name="department"
                    value={formData.department}
                    onChange={handleChange}
                    required
                    style={{ width: "100%", padding: 12, border: "1px solid #d1d5db", borderRadius: 6 }}
                >
                    {DEPARTMENTS.map((d) => (
                        <option key={d} value={d}>{d}</option>
                    ))}
                </select>

                <label htmlFor="password">Password</label>
                <input
                    id="password"
                    type="password"
                    name="password"
                    placeholder="Create a password (minimum 8 characters)"
                    value={formData.password}
                    onChange={handleChange}
                    minLength={8}
                    maxLength={128}
                    autoComplete="new-password"
                    required
                />
                <small className="field-hint">Use at least 8 characters, including uppercase and lowercase letters, a number, and a special character.</small>

                <label htmlFor="confirmPassword">Confirm Password</label>
                <input
                    id="confirmPassword"
                    type="password"
                    name="confirmPassword"
                    placeholder="Confirm your password"
                    value={formData.confirmPassword}
                    onChange={handleChange}
                    minLength={8}
                    maxLength={128}
                    autoComplete="new-password"
                    required
                />

                <button type="submit" disabled={loading}>
                    {loading ? "Creating Account..." : "Register"}
                </button>

                <p className="auth-footer">
                    Already have an account? <Link to="/login">Login</Link>
                </p>
            </form>
        </div>
    );
}
