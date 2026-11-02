import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import ProtectedRoute from "./components/ProtectedRoute";
import Layout from "./components/Layout";
import Login from "./pages/Login";
import Register from "./pages/Register";
import Dashboard from "./pages/Dashboard";
import Events from "./pages/Events";
import Applications from "./pages/Applications";
import MyApplications from "./pages/MyApplications";
import AdminTeachers from "./pages/AdminTeachers";

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Navigate to="/login" replace />} />
        <Route path="/login" element={<Login />} />
        <Route path="/register" element={<Register />} />
        <Route element={<Layout />}>
          <Route
            path="/dashboard"
            element={
              <ProtectedRoute>
                <Dashboard />
              </ProtectedRoute>
            }
          />
          <Route
            path="/events"
            element={
              <ProtectedRoute>
                <Events />
              </ProtectedRoute>
            }
          />
          <Route
            path="/applications"
            element={
              <ProtectedRoute roles={["Admin", "Teacher"]}>
                <Applications />
              </ProtectedRoute>
            }
          />
          <Route
            path="/my-applications"
            element={
              <ProtectedRoute roles={["Student"]}>
                <MyApplications />
              </ProtectedRoute>
            }
          />
          <Route
            path="/admin/teachers"
            element={
              <ProtectedRoute roles={["Admin"]}>
                <AdminTeachers />
              </ProtectedRoute>
            }
          />
        </Route>
      </Routes>
    </BrowserRouter>
  );
}

export default App;
