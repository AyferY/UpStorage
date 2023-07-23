import './App.css'
import NavBar from './components/NavBar.tsx'
import {Container} from "semantic-ui-react";
import { Route, Routes } from 'react-router-dom';
import Log from './pages/Log.tsx';
import NotFoundPage from './pages/NotFoundPage.tsx';
import Order from './pages/Order.tsx';

function App() {

  
  return (
    <>
    <NavBar/>
    <Container className="App">
      <Routes>
        <Route path="/order" element={<Order />} />
        <Route path="*" element={<NotFoundPage />} />
        <Route path="/log" element={<Log />} />
      </Routes>
    </Container>
    
    </>
  )
}

export default App
