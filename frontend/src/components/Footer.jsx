export default function Footer() {
    return (
        <footer className="footer">
            <div className="footer-content">
                <span className="footer-logo">🎓 College<span className="footer-accent">Event</span></span>
                <p className="footer-text">
                    &copy; {new Date().getFullYear()} College Event Management System. All rights reserved.
                </p>
                <div className="footer-links">
                    <span>Admin • Teacher • Student</span>
                </div>
            </div>
        </footer>
    );
}