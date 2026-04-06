import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import axios from "axios";
import SideBar from "./SideBarComponent";


function LoginComponent()
{

        const [username, setUsername] = useState("");
        const [password, setPassword] = useState("");
        const [remember, setRemember] = useState(false);
        const [error, setError] = useState("");
        const [status, setStatus] = useState(false);

        const apiUrl = import.meta.env.VITE_API_URL;
        const navigate = useNavigate();

        const token = localStorage.getItem("token") || sessionStorage.getItem("token");

        const handleLogin = async () => {
                if(!username || !password)
                {
                        setError("The fields are empty.");
                        return 0;
                } else {
                        try
                        {       
                                setStatus(true);
                                const response = await axios.post(`${apiUrl}/api/authentication/login`, {Identification: username, Password: password});

                                const token = response.data.token;

                                if(remember)
                                {
                                        localStorage.setItem("token", token);
                                } else {
                                        sessionStorage.setItem("token", token);
                                }

                                navigate("/profile")
                        }       
                        catch(ex)
                        {
                                var errorMessage = ex.response.data.error;

                                if(errorMessage == "Your account hasn't been activated.")
                                {
                                        sessionStorage.setItem("activateUser", "true");
                                        navigate("/profile/activate_message");
                                } else {
                                        setError(errorMessage);
                                }
                        }
                        finally
                        {
                                setStatus(false);
                        }
                }
        };

        return (
                <div className="flex flex-col h-screen items-center m-4">
                        <SideBar />

                        <div className="flex flex-1 w-full items-center justify-center">

                                <div className="flex flex-col gap-8">

                                        {error && ( <div className="flex bg-red-200 p-4 rounded-lg">
                                                <span className="text-red-500 font-semibold">{error}</span>
                                        </div>
                                        )}

                                        <div className="flex flex-col bg-amber-300 p-6 rounded-lg w-96 gap-4">

                                                <span className="text-center text-2xl font-mono font-semibold">
                                                        Sign In
                                                </span>

                                                <div className="flex flex-col gap-2">
                                                        <label className="text-lg font-mono">
                                                                Username
                                                        </label>

                                                        <input
                                                                type="text"
                                                                className="w-full rounded-lg p-2 border border-stone-400 outline-none focus:ring-2 focus:ring-amber-500"
                                                                onChange={(e) => {setUsername(e.target.value)}}
                                                        />
                                                </div>

                                                <div className="flex flex-col gap-2">
                                                        <label className="text-lg font-mono">
                                                                Password
                                                        </label>

                                                        <input
                                                                type="password"
                                                                className="w-full rounded-lg p-2 border border-stone-400 outline-none focus:ring-2 focus:ring-amber-500"
                                                                onChange={(e) => {setPassword(e.target.value)}}
                                                        />
                                                </div>

                                                <label className="flex items-center gap-2 text-sm font-mono cursor-pointer">
                                                
                                                <input
                                                        type="checkbox"
                                                        className="w-4 h-4 accent-amber-500"
                                                        onChange={(e) => {setRemember(e.target.checked)}}
                                                />
                                                        Remember me
                                                </label>

                                                <button className="rounded-lg p-2 bg-amber-500 hover:bg-amber-600 text-white font-semibold transition" onClick={handleLogin} disabled={status}>
                                                {status ? "Wait..." : "Sign In"}
                                                </button>
                                                
                                                <span className="text-center font-semibold text-gray-700">Don't have an account? <button onClick={() => navigate("/register")}>Register</button>.</span>
                                        </div>
                                        
                                </div>
                        </div>
                </div>
        );
}

export default LoginComponent;