const partieEndpoint = "http://localhost:5286/ApiParties/Parties";

class PartieApiService {
  // Récupérer toutes les parties
  async getAllParties() {
    return this.fetchFromApi(`${partieEndpoint}`, "GET");
  }

  // Récupérer une partie par son ID
  async getPartieById(id) {
    return this.fetchFromApi(`${partieEndpoint}/${id}`, "GET");
  }

  // Récupérer toutes les parties de l'utilisateur connecté
  async getMyParties() {
    return this.fetchFromApi(`${partieEndpoint}/my-parties`, "GET");
  }

  // Créer une partie
  async createPartie(payload) {
    if (!payload.code) {
      payload.code = Math.random().toString(36).slice(2, 7).toUpperCase();
    }
    return this.fetchFromApi(`${partieEndpoint}/create`, "POST", payload);
  }

  // Rejoindre une partie
  async joinPartie(code) {
    return this.fetchFromApi(`${partieEndpoint}/join`, "POST", { code });
  }

  // Supprimer une partie
  async deletePartie(id) {
    return this.fetchFromApi(`${partieEndpoint}/${id}`, "DELETE");
  }

  // Récupérer le destinataire du tirage pour l'utilisateur connecté
  async getMonDestinataire(partieId) {
    return this.fetchFromApi(`${partieEndpoint}/${partieId}/mon-destinataire`, "GET");
  }


  // Fonction générique pour appeler l'API
  async fetchFromApi(url, method = "GET", body = null) {
    console.log(`📡 Fetching API: ${method} ${url}`);
    try {
      const token = localStorage.getItem("token"); // Récupère le token JWT
      const response = await fetch(url, {
        method,
        headers: {
          "Content-Type": "application/json",
          Authorization: token ? `Bearer ${token}` : "",
        },
        body: body ? JSON.stringify(body) : null,
      });

      // Vérifie si la réponse contient du JSON
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
