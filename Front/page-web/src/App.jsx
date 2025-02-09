import React from "react";
import Header from "./Components/Header";
import Principale from "./Pages/Principale-nonco";
import './App.css'

function App() {
  return (
    <div>
      <Header />
      <main>
        <Principale />
      </main>
    </div>
  );
}

export default App;