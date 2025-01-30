// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

async function deleteMessage(messageID) {
    const confirmed = confirm("Are you sure you want to delete this message?");
    if (confirmed) {
        try {
            const response = await fetch(`/Chat?handler=DeleteMessage`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ messageID: messageID }) // Proper JSON format
            });

            if (response.ok) {
                alert("Message deleted successfully.");
                location.reload(); // Refresh to reflect changes
            } else if (response.status === 404) {
                alert("Message not found.");
            } else {
                alert("Failed to delete message.");
            }
        } catch (error) {
            console.error("Error deleting message:", error);
            alert("An error occurred while deleting the message.");
        }
    }
}