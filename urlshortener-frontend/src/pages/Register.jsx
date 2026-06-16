import { useState } from "react";
import api from "../services/api";

function Register() {

  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");

  async function handleRegister() {

    try {

      await api.post("/auth/register", {
        username,
        password
      });

      alert("Registration Successful");

    } catch (error) {

      console.log(error.response?.data);

      alert(
        error.response?.data || "Registration Failed"
      );
    }
  }

  return (
    <div>

      <h1>Register</h1>

      <input
        type="text"
        placeholder="Username"
        onChange={(e) => setUsername(e.target.value)}
      />

      <br />
      <br />

      <input
        type="password"
        placeholder="Password"
        onChange={(e) => setPassword(e.target.value)}
      />

      <br />
      <br />

      <button onClick={handleRegister}>
        Register
      </button>

    </div>
  );
}

export default Register;