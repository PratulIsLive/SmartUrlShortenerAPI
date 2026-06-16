import { Link } from "react-router-dom";

function Navbar() {
  return (
    <nav>
      <Link to="/">Home</Link>

      {" | "}

      <Link to="/login">Login</Link>

      {" | "}

      <Link to="/register">Register</Link>

      {" | "}

      <Link to="/dashboard">Dashboard</Link>

      {" | "}

      <Link to="/myurls">My URLs</Link>
    </nav>
  );
}

export default Navbar;