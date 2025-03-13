const rootEndpoint = "http://localhost:5286/ApiParties/Parties"; // 🔥 Adapte à ton backend

class PartieApiService {
  // 🔹 Récupérer toutes les parties
  async getAllParties() {
    return this.fetchFromApi(`${rootEndpoint}`, "GET");
  }

  // 🔹 Récupérer une partie par son ID
  async getPartieById(id) {
    return this.fetchFromApi(`${rootEndpoint}/${id}`, "GET");
  }

  // 🔹 Créer une partie (l'utilisateur connecté est automatiquement admin)
  async createPartie(name) {
    const code = Math.random().toString(36).slice(2, 8).toUpperCase(); // ✅ Utilisation de slice()
    return this.fetchFromApi(`${rootEndpoint}/create`, "POST", { code, name }); // ✅ Envoie "Code" et "Name"
  }

  // 🔹 Rejoindre une partie
  async joinPartie(id) {
    return this.fetchFromApi(`${rootEndpoint}/${id}/join`, "POST");
  }

  // 🔹 Supprimer une partie (seulement l’admin peut le faire)
  async deletePartie(id) {
    return this.fetchFromApi(`${rootEndpoint}/${id}`, "DELETE");
  }

  // 🔹 Fonction générique pour appeler l'API avec gestion des erreurs
  async fetchFromApi(url, method = "GET", body = null) {
    console.log(`📡 Fetching API: ${method} ${url}`);
    try {
      const token = localStorage.getItem("token"); // 🔥 Récupère le token JWT

      const response = await fetch(url, {
        method,
        headers: {
          "Content-Type": "application/json",
          Authorization: token ? `Bearer ${token}` : "", // 🔥 Envoi du token
        },
        body: body ? JSON.stringify(body) : null,
      });

      // 🔍 Vérifie si la réponse contient du JSON
      const contentType = response.headers.get("content-type");
      let content;
      if (contentType && contentType.includes("application/json")) {
        content = await response.json();
      } else {
        content = await response.text();
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

export default new PartieApiService();
