import React, {useState} from "react";
import { useNavigate } from "react-router-dom";
import axios from "axios";
import SideBar from "./SideBarComponent";

function RegisterComponent()
{
        const [username, setUsername] = useState("");
        const [email, setEmail] = useState("");
        const [password, setPassword] = useState("");
        
        const [status, setStatus] = useState(false);
        const [error, setError] = useState("");

        const apiUrl = import.meta.env.VITE_API_URL;
        const navigate = useNavigate();
        
        var token = sessionStorage.getItem("token");
        
        if (!token) {
                token = localStorage.getItem("token");
        }

        const handleRegister = async () => {

                if(!username || !email || !password)
                {
                        setError("The fields are empty.");
                        return 0;
                } else {
                        try
                        {
                                setStatus(true);
                                
                                const response = await axios.post(`${apiUrl}/api/authentication/register`, {Username: username, Email: email, Password: password});

                                sessionStorage.setItem("activateUser", "true");
                                navigate("/profile/activate_message");
                        }
                        catch(ex)
                        {
                                var errorMessage = ex.response.data.error;

                                setError(errorMessage);
                        }
                        finally
                        {
                                setStatus(false);
                        }
                }
        }

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
                                                        Sign Up
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
                                                                Email
                                                        </label>

                                                        <input
                                                                type="email"
                                                                className="w-full rounded-lg p-2 border border-stone-400 outline-none focus:ring-2 focus:ring-amber-500"
                                                                onChange={(e) => {setEmail(e.target.value)}}
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

                                                <button className="rounded-lg p-2 bg-amber-500 hover:bg-amber-600 text-white font-semibold transition" onClick={handleRegister} disabled={status}>
                                                {status ? "Wait..." : "Sign Up"}
                                                </button>

                                        </div>
                                </div>
                        </div>
                </div>
        );
}

export default RegisterComponent;