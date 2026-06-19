import { useState } from "react";
import api from "../services/api";

function Home() {

    const [originalUrl, setOriginalUrl] = useState("");
    const [customCode, setCustomCode] = useState("");
    const [result, setResult] = useState(null);

    const shortenUrl = async () => {

        try {

            const response = await api.post(
                "/url/shorten",
                {
                    originalUrl,
                    customCode
                },
                {
                    headers: {
                        Authorization:
                            `Bearer ${localStorage.getItem("token")}`
                    }
                }
            );

            setResult(response.data);

        }
        catch (error) {

            console.log(error.response?.data);

            alert(
                error.response?.data ||
                "Unable to shorten URL"
            );

        }
    };

    const copyToClipboard = () => {

        navigator.clipboard.writeText(
            result.shortUrl
        );

        alert("Short URL copied");

    };

    return (
        <div>

            <h1>Smart URL Shortener</h1>

            <input
                type="text"
                placeholder="Enter long URL"
                value={originalUrl}
                onChange={(e) =>
                    setOriginalUrl(e.target.value)
                }
            />

            <br />
            <br />

            <input
                type="text"
                placeholder="Custom code (optional)"
                value={customCode}
                onChange={(e) =>
                    setCustomCode(e.target.value)
                }
            />

            <br />
            <br />

            <button onClick={shortenUrl}>
                Shorten URL
            </button>

            {
                result &&
                <>
                    <h3>Short URL</h3>

                    <a
                        href={result.shortUrl}
                        target="_blank"
                        rel="noreferrer"
                    >
                        {result.shortUrl}
                    </a>

                    <br />
                    <br />

                    <button onClick={copyToClipboard}>
                        Copy
                    </button>

                    <br />
                    <br />

                    <h3>QR Code</h3>

                    <img
                        src={result.qrCodeUrl}
                        alt="QR Code"
                        width="200"
                    />

                </>
            }

        </div>
    );
}

export default Home;