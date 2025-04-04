import React from "react";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import Header from "./components/Header";
import Principale from "./Pages/PrincipaleNonCo";
import PrincipaleCo from "./Pages/PrincipaleCo";
import Connection from "./Pages/Connection"
import Inscription from "./Pages/Inscription";
import PartieDetails from "./Pages/PartieDetails";
import MesGroupes from "./Pages/MesGroupes";
import MonProfil from "./Pages/Profil"

function App() {
  const token = localStorage.getItem("token"); 

  return (
    <Router>
      <Header />
      <Routes>
        <Route path="/" element={token ? <PrincipaleCo /> : <Principale />} />
        <Route path="/login" element={<Connection />} />
        <Route path="/register" element={<Inscription />} /> 
        <Route path="/partie/:id" element={<PartieDetails />} />
        <Route path="/mes-groupes" element={<MesGroupes />} /> 
        <Route path="/profil" element={<MonProfil />} />
      </Routes>
    </Router>
  );
}

export default App;