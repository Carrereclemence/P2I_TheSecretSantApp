import React from "react";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import Header from "./components/Header";
import Principale from "./Pages/PrincipaleNonCo";
import PrincipaleCo from "./Pages/PrincipaleCo";
import Connection from "./Pages/Connection"

function App() {
  const token = localStorage.getItem("token"); 

  return (
    <Router>
      <Header />
      <Routes>
        <Route path="/" element={token ? <PrincipaleCo /> : <Principale />} />
        <Route path="/login" element={<Connection />} />
      </Routes>
    </Router>
  );
}

export default App;