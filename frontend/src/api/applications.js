import { fetchWithAuth } from "./auth";

export async function submitApplication(payload) {
    return fetchWithAuth("/applications", {
        method: "POST",
        body: JSON.stringify(payload),
    });
}

export async function getMyApplications() {
    return fetchWithAuth("/applications/my");
}

export async function getAllApplications() {
    return fetchWithAuth("/applications");
}

export async function reviewApplication(id, payload) {
    return fetchWithAuth(`/applications/${id}/status`, {
        method: "PUT",
        body: JSON.stringify(payload),
    });
}
