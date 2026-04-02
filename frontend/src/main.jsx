import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import { BrowserRouter, Route, Routes } from 'react-router-dom';
import LoginComponent from './Component/LoginComponent';
import ActivateMessage from './Component/Profile/ActivateMessageComponent';
import RegisterComponent from './Component/RegisterComponent';
import ActivateAccount from './Component/Profile/VerifyComponent';

createRoot(document.getElementById('root')).render(
  <StrictMode>
    <BrowserRouter>
      <Routes>
        <Route path="/login" element={<LoginComponent />} />
        <Route path="/register" element={<RegisterComponent />} />
        <Route path="/profile/activate_message" element={<ActivateMessage />} />
        <Route path="/profile/activate_account" element={<ActivateAccount />} />
      </Routes>
    </BrowserRouter>
  </StrictMode>
)