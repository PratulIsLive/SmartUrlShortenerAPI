import { useEffect, useState } from "react";
import api from "../services/api";

function Dashboard() {

    const [dashboard, setDashboard] = useState(null);

    useEffect(() => {

        async function loadDashboard() {

            try {

                const response = await api.get(
                    "/url/dashboard",
                    {
                        headers: {
                            Authorization:
                                `Bearer ${localStorage.getItem("token")}`
                        }
                    }
                );

                setDashboard(response.data);

            }
            catch {

                alert("Unable to load dashboard");
            }
        }

        loadDashboard();

    }, []);

    if (!dashboard) {

        return <h1>Loading...</h1>;
    }

    return (
        <div>

            <h1>Dashboard</h1>

            <h2>
                Total URLs: {dashboard.totalUrls}
            </h2>

            <h2>
                Total Visits: {dashboard.totalVisits}
            </h2>

            <h2>
                Active URLs: {dashboard.activeUrls}
            </h2>

            <h2>
                Expired URLs: {dashboard.expiredUrls}
            </h2>

            <h2>
                Most Visited Short Code:
                {" "}
                {dashboard.mostVisitedShortCode ?? "None"}
            </h2>

        </div>
    );
}

export default Dashboard;