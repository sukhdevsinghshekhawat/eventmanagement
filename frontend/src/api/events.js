import { fetchWithAuth } from "./auth";

export async function getEvents() {
    return fetchWithAuth("/events");
}

export async function getEvent(id) {
    return fetchWithAuth(`/events/${id}`);
}

export async function createEvent(payload) {
    return fetchWithAuth("/events", {
        method: "POST",
        body: JSON.stringify(payload),
    });
}

export async function updateEvent(id, payload) {
    return fetchWithAuth(`/events/${id}`, {
        method: "PUT",
        body: JSON.stringify(payload),
    });
}

export async function deleteEvent(id) {
    return fetchWithAuth(`/events/${id}`, { method: "DELETE" });
}
