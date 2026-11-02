
import { useState } from "react";
import { useNavigate, Link } from "react-router-dom";
import { loginUser } from "../api/auth";

export default function Login() {
    const navigate = useNavigate();

    const [form, setForm] = useState({
        email: "",
        password: "",
    });

    const [error, setError] = useState("");
    const [loading, setLoading] = useState(false);

    function handleChange(event) {
        setForm({
            ...form,
            [event.target.name]: event.target.value,
        });
    }

    async function handleSubmit(event) {
        event.preventDefault();
        setError("");
        setLoading(true);

        try {
            const data = await loginUser(form);

            localStorage.setItem("accessToken", data.accessToken);

            navigate("/dashboard");
        } catch (err) {
            setError(err.message || "Something went wrong.");
        } finally {
            setLoading(false);
        }
    }

    return (
        <div className="login-page">
            <form className="login-card" onSubmit={handleSubmit}>
                <h1>College Event Management</h1>
                <p>Login to your account</p>

                {error && <div className="error-message" role="alert">{error}</div>}

                <label htmlFor="email">Email</label>
                <input
                    id="email"
                    name="email"
                    type="email"
                    value={form.email}
                    onChange={handleChange}
                    placeholder="Enter your email"
                    autoComplete="username"
                    maxLength={256}
                    required
                />

                <label htmlFor="password">Password</label>
                <input
                    id="password"
                    name="password"
                    type="password"
                    value={form.password}
                    onChange={handleChange}
                    placeholder="Enter your password"
                    autoComplete="current-password"
                    maxLength={128}
                    required
                />

                <button type="submit" disabled={loading}>
                    {loading ? "Logging in..." : "Login"}
                </button>
            </form>

            <p className="auth-footer">
                Don't have an account? <Link to="/register">Register</Link>
            </p>

        </div>
    );
}
