window.sendRequestPOST = async function (url, body) {
    const response =  await fetch(url, {
        method: "POST",
        credentials: "include",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(body)
    });

    if (!response.ok) {
        throw new Error("HTTP error " + response.status);
    }

    return await response.json();
};
