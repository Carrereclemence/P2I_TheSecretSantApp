const rootEndpoint = "http://localhost:5286/ApiUsers/Users";

// Modèle utilisateur
export class User {
  constructor(id, userName, firstName, lastName, admin) {
    this.id = id;
    this.userName = userName;
    this.firstName = firstName;
    this.lastName = lastName;
    this.admin = admin;
  }
}

class ApiService {
  // 🔹 Récupérer les infos de l'utilisateur connecté
  async getCurrentUser() {
    return this.fetchFromApi(`${rootEndpoint}/me`, "GET");
  }

  // 🔹 Connexion de l'utilisateur
  async login(userName, password) {
    const data = await this.fetchFromApi(`${rootEndpoint}/login`, "POST", {
      userName,
      password,
    });

    if (data?.token) {
      localStorage.setItem("token", data.token);
    }

    return data;
  }

  // 🔹 Déconnexion de l'utilisateur
  logout() {
    localStorage.removeItem("token");
  }

  // 🔹 Inscription d'un nouvel utilisateur
  async register(user) {
    return this.fetchFromApi(`${rootEndpoint}/register`, "POST", user);
  }

  // 🔹 Récupérer tous les utilisateurs (Admin uniquement)
  async getAllUsers() {
    return this.fetchFromApi(`${rootEndpoint}`, "GET");
  }

  // 🔹 Récupérer un utilisateur par son ID (Admin uniquement)
  async getUserById(id) {
    return this.fetchFromApi(`${rootEndpoint}/${id}`, "GET");
  }

  // 🔹 Mettre à jour un utilisateur
  async updateUser(id, user) {
    return this.fetchFromApi(`${rootEndpoint}/${id}`, "PUT", user);
  }

  // 🔹 Supprimer un utilisateur (Admin uniquement)
  async deleteUser(id) {
    return this.fetchFromApi(`${rootEndpoint}/${id}`, "DELETE");
  }

  // 🔹 Fonction générique pour faire des requêtes à l'API
  async fetchFromApi(url, method = "GET", body = null) {
    console.log(`📡 Fetching API: ${method} ${url}`);
    try {
      const token = localStorage.getItem("token"); // 🔥 Récupère le token si dispo

      const response = await fetch(url, {
        method,
        headers: {
          "Content-Type": "application/json",
          Authorization: token ? `Bearer ${token}` : "", // 🔥 Envoi du token si présent
        },
        body: body ? JSON.stringify(body) : null,
      });

      // 🔍 Vérifie si la réponse est vide ou non
      const contentType = response.headers.get("content-type");
      let content;
      if (contentType && contentType.includes("application/json")) {
        content = await response.json();
      } else {
        content = await response.text(); // Si la réponse n'est pas JSON
      }

      if (!response.ok) {
        throw new Error(content.message || "Erreur API");
      }

      return content;
    } catch (error) {
      console.error("❌ Erreur API :", error);
      throw error;
    }
  }
}

export default new ApiService();
