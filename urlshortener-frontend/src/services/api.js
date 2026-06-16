import axios from "axios";

const api = axios.create({
  baseURL: "https://smarturlshortenerapi.onrender.com/api"
});

export default api;