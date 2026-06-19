import { useEffect, useState } from "react";
import api from "../services/api";

function MyUrls() {

  const [urls, setUrls] = useState([]);

  useEffect(() => {
    fetchUrls();
  }, []);

  async function fetchUrls() {

    try {

      const response = await api.get(
        "/url/all",
        {
          headers: {
            Authorization:
              `Bearer ${localStorage.getItem("token")}`
          }
        }
      );

      setUrls(response.data.data);

    } catch {

      alert("Unable to fetch URLs");
    }
  }

  async function deleteUrl(shortCode) {

    try {

      await api.delete(
        `/url/${shortCode}`,
        {
          headers: {
            Authorization:
              `Bearer ${localStorage.getItem("token")}`
          }
        }
      );

      fetchUrls();

    } catch {

      alert("Delete failed");
    }
  }

  return (

    <div>

      <h1>My URLs</h1>

      {
        urls.map((url) => (

          <div
            key={url.shortCode}
            style={{
              border: "1px solid gray",
              padding: "15px",
              marginBottom: "20px"
            }}
          >

            <p>
              <strong>Original URL:</strong>
              {" "}
              {url.originalUrl}
            </p>

            <p>
              <strong>Short Code:</strong>
              {" "}
              {url.shortCode}
            </p>

            <p>
              <strong>Visits:</strong>
              {" "}
              {url.visitCount}
            </p>

            <button
              onClick={() =>
                deleteUrl(url.shortCode)
              }
            >
              Delete
            </button>

          </div>

        ))
      }

    </div>

  );
}

export default MyUrls;