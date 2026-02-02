import { initializeApp } from "https://www.gstatic.com/firebasejs/10.7.1/firebase-app.js";
import { getDatabase, ref, onValue, get } from "https://www.gstatic.com/firebasejs/10.7.1/firebase-database.js";

// 1. Initialize with the Official HN Database URL
const app = initializeApp({
    databaseURL: "https://hacker-news.firebaseio.com",
});

const db = getDatabase(app);
// 3. Reference the 'maxitem' endpoint (the ID of the newest item)
const maxItemRef = ref(db, "v0/maxitem");
console.log("Listening for updates...");

onValue(maxItemRef, async (snapshot) => {
    const newId = snapshot.val();

    if (newId) {
        // 5. Fetch the details for this new ID
        const itemRef = ref(db, `v0/item/${newId}`);
        const itemSnapshot = await get(itemRef);
        const item = itemSnapshot.val();

        // Render the result
        const type = item.type || "unknown";
        const text = item.title || item.text || "(no text)";
        const author = item.by || "anon";

        const log = `[${type.toUpperCase()}] ID:${newId} by @${author}: ${text.substring(0, 50)}...`;

        const p = document.createElement('div');
        p.innerText = log;
        //feedDiv.prepend(p); // Add new items to the top
    }
});
