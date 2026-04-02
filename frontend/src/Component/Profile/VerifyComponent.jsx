import React, {useEffect, useState} from "react";
import { useNavigate, useSearchParams } from "react-router-dom";
import axios from "axios";
import SideBar from "../SideBarComponent";

function ActivateAccount()
{
        const [success, setSuccess] = useState(null);
        const [tried, setTried] = useState(false);

        const [searchParams] = useSearchParams();

        const apiUrl = import.meta.env.VITE_API_URL;
        const navigate = useNavigate();

        useEffect(() => {
                const t = searchParams.get("token");
                        
                if(!t)
                {
                        navigate("/login");
                } else {

                const handleVerify = async () => {
                        try
                        {
                                const response = await axios.post(`${apiUrl}/api/authentication/verify`, {Token: t});
                                setSuccess(true);
                        }
                        catch
                        {
                                setSuccess(false);
                        }

                        setTried(true);
                };

                handleVerify();
        }
        }, [searchParams, navigate, apiUrl]);

        const handleBack = () => {
                navigate("/login")
        }

        return (
                <div className="flex flex-col h-screen items-center m-4">
                        <SideBar />                             
                        
                        {tried && (
                                success ? (
                                        <div className="flex flex-1 w-full items-center justify-center">
                                        
                                        <div className="flex flex-col items-center justify-center bg-amber-300 p-2 m-2 rounded-lg w-96">
                                                <span className="font-semibold text-xl text-gray-800">Your account has been activated.</span>
                                                <span className="font-semibold text-gray-800 text-center">Your account has been successfully activated. From now on, you can log in.</span>

                                                <button className="bg-amber-500 rounded-lg p-2 m-2 font-semibold text-gray-800" onClick={handleBack}>Back to login</button>
                                        </div>

                                        </div>
                                ) : (

                                        <div className="flex flex-1 w-full items-center justify-center">
                                        
                                        <div className="flex flex-col items-center justify-center bg-amber-300 p-2 m-2 rounded-lg w-96">
                                                <span className="font-semibold text-xl text-gray-800">Your account hasn't been activated.</span>
                                                <span className="font-semibold text-gray-800 text-center">Your token has expired or is invalid. Please log in and try again.</span>

                                                <button className="bg-amber-500 rounded-lg p-2 m-2 font-semibold text-gray-800" onClick={handleBack}>Back to login</button>
                                        </div>

                                        </div>
                                )
                        )}

                        {!tried && (
                                <>
                                        
                                </>
                        )}
                </div>
        );
}

export default ActivateAccount;