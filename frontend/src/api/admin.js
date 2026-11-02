import { fetchWithAuth } from "./auth";

export async function createTeacher(payload) {
    return fetchWithAuth("/admin/teachers", {
        method: "POST",
        body: JSON.stringify(payload),
    });
}

export async function getTeachers() {
    return fetchWithAuth("/admin/teachers");
}

export async function deleteTeacher(id) {
    return fetchWithAuth(`/admin/teachers/${id}`, { method: "DELETE" });
}

export async function getStudents() {
    return fetchWithAuth("/admin/students");
}

export async function setUserStatus(id, isActive) {
    return fetchWithAuth(`/admin/users/${id}/status`, {
        method: "PATCH",
        body: JSON.stringify({ isActive }),
    });
}
