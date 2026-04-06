import axios from "axios";
import React, { useState } from "react";

import { MdOutlineEdit, MdEmail, MdMail } from "react-icons/md";
import { IoArrowBackCircleOutline } from "react-icons/io5";
import { useNavigate } from "react-router-dom";


function MyProfile({initialUsername, initialEmail})
{
        const [username, setUsername] = useState(initialUsername);
        const [email, setEmail] = useState(initialEmail);

        const [usernamePOPUP, setUsernamePOPUP] = useState();
        const [emailPOPUP, setEmailPOPUP] = useState();
        const [passwordPOPUP, setPasswordPOPUP] = useState();
        
        const [currentPassword, setCurrentPassword] = useState("");

        const [newParam, setNewParam] = useState("");

        const [error, setError] = useState("");
        const [success, setSuccess] = useState("");

        const apiUrl = import.meta.env.VITE_API_URL;
        const token = localStorage.getItem("token") || sessionStorage.getItem("token");

        const navigate = useNavigate();

        const handleChangeUsername = async () => {
                
                if(!newParam || !currentPassword)
                {
                        setError("The fields are empty.");
                        return;
                }

                try
                {
                        const response = await axios.post(
                                `${apiUrl}/api/profile/change_username`,
                                { Username: newParam, Password: currentPassword },
                                {
                                        headers: {
                                                Authorization: `Bearer ${token}`,
                                        },
                                }
                        );
                        
                        setUsername(newParam);
                        setSuccess(true);
                }

                catch(ex)
                {
                        setError(ex.response.data.error);
                }
        };

        const handleChangeEmail = async () => {
                
                if(!newParam || !currentPassword)
                {
                        setError("The fields are empty.");
                        return;
                }

                try
                {
                        const response = await axios.post(
                                `${apiUrl}/api/profile/change_email`,
                                { Email: newParam, Password: currentPassword },
                                {
                                        headers: {
                                                Authorization: `Bearer ${token}`,
                                        },
                                }
                        );

                        setSuccess(true);
                }

                catch(ex)
                {
                        setError(ex.response.data.error);
                        setSuccess(false);
                }
        }

        const handleChangePassword = async () => {
                if(!newParam || !currentPassword)
                {
                        setError("The fields are empty.");
                        return 0;
                }

                try
                {
                        const response = await axios.post(
                                `${apiUrl}/api/profile/change_password`,
                                {NewPassword: newParam, Password: currentPassword},
                                {
                                        headers: {
                                                        Authorization: `Bearer ${token}`,
                                        },
                                }
                        );

                        localStorage.clear();
                        sessionStorage.clear();

                        navigate("/login");
                }
                catch(ex)
                {
                        setError(ex.response.data.error);
                }
        }

        return (
                <>
                        <div className="flex flex-col items-center p-4 w-full rounded-lg">
                                <span className="text-lg font-semibold">{username}</span>
                                <span className="text-xs text-gray-700 mt-1">
                                        {/* 16 recipes • 23 favourite recipes • 53 karma */}
                                </span>
                        </div>
                
                        <div className="mt-4 space-y-3 w-full">
                                <div className="flex flex-col p-3 rounded-lg bg-amber-300 shadow-sm">
                                        <span className="text-xs text-gray-500">Username</span>
                                        <div>
                                                <span className="font-semibold">{username}</span>
                                                <button className="m-2" onClick={() => setUsernamePOPUP(true)}> 
                                                        <MdOutlineEdit />
                                                </button>
                                        </div>
                                </div>
                
                                <div className="flex flex-col p-3 rounded-lg bg-amber-300 shadow-sm">
                                        <span className="text-xs text-gray-500 font-semibold">Email address</span>
                                        
                                        <div>
                                                <span className="font-semibold">{email}</span>
                                                <button className="m-2" onClick={() => setEmailPOPUP(true)}>
                                                        <MdOutlineEdit />
                                                </button>
                                        </div>
                                </div>
                
                                <button className="bg-amber-300 p-2 rounded-lg" onClick={() => setPasswordPOPUP(true)}>Change your password</button>
                        </div>

                        {emailPOPUP && (
                                <div className="fixed inset-0 bg-black/50 flex flex-col items-center justify-center z-50">

                                {error && (<div className="flex flex justify-center bg-red-300 rounded-lg w-96 p-2 m-4">
                                        <span className="text-center text-red-800">{error}</span>
                                </div> )}

                                <div className="flex flex-col bg-amber-300 p-2 rounded-lg w-96">
                                        <button className="w-4 items-center" onClick={() => { setEmailPOPUP(false); setNewParam(""); setCurrentPassword(""); setError(""); setSuccess(false);}}>
                                                <IoArrowBackCircleOutline size={20}/>
                                        </button>

                                        {!success && (
                                                <>

                                                        <span className="font-semibold w-full text-center block m-2 text-lg">Change your email address</span>

                                                        <span className="font-semibold m-2">New email address</span>
                                                        <input type="email" className="p-2 m-2 rounded-lg" value={newParam} onChange={(e) => {setNewParam(e.target.value)}}/>

                                                        <span className="font-semibold m-2">Password</span>
                                                        <input type="password" className="p-2 m-2 rounded-lg" value={currentPassword} onChange={(e) => {setCurrentPassword(e.target.value)}}/>

                                                        <button className="bg-amber-200 p-2 m-2 rounded-lg font-semibold text-gray-700" onClick={handleChangeEmail}>
                                                                Change email
                                                        </button>

                                                </>
                                        )}

                                        {success && (
                                                <div className="flex flex-col justify-center items-center">
                                                        <span className="text-semibold text-center text-lg">We will send a confirmation email to {newParam}.</span>
                                                        <MdMail size={80} />
                                                </div>)}
                                        </div>
                                </div>
                                )}

                                {usernamePOPUP && (
                                        
                                        <div className="fixed inset-0 bg-black/50 flex flex-col items-center justify-center z-50">

                                                {!success && (
                                                <>

                                                        {error && (<div className="flex flex justify-center bg-red-300 rounded-lg w-96 p-2 m-4">
                                                                <span className="text-center text-red-800">{error}</span>
                                                        </div> )}

                                                        <div className="flex flex-col bg-amber-300 p-2 rounded-lg w-96">
                                                                        
                                                                <button className="w-4 items-center" onClick={() => { setUsernamePOPUP(false); setNewParam(""); setCurrentPassword(""); setError(""); setSuccess(false);}}>
                                                                        <IoArrowBackCircleOutline size={20}/>
                                                                </button>

                                                                <span className="font-semibold w-full text-center block m-2 text-lg">Change your username</span>

                                                                        <span className="font-semibold m-2">New username</span>
                                                                        <input type="text" className="p-2 m-2 rounded-lg" value={newParam} onChange={(e) => {setNewParam(e.target.value)}}/>

                                                                        <span className="font-semibold m-2">Password</span>
                                                                        <input type="password" className="p-2 m-2 rounded-lg" value={currentPassword} onChange={(e) => {setCurrentPassword(e.target.value)}}/>

                                                                        <button className="bg-amber-200 p-2 m-2 rounded-lg font-semibold text-gray-700" onClick={handleChangeUsername}>
                                                                                Change username         
                                                                        </button>
                                                                </div>
                                                </>
                                                )}

                                                {success && (
                                                        <>
                                                                <div className="flex flex-col bg-amber-300 p-2 rounded-lg w-96">
                                                                                
                                                                        <button className="w-4 items-center" onClick={() => { setUsernamePOPUP(false); setNewParam(""); setCurrentPassword(""); setError(""); setSuccess(false);}}>
                                                                                <IoArrowBackCircleOutline size={20}/>
                                                                        </button>

                                                                        <span className="font-semibold w-full text-center block m-2 text-lg">Your username was changed.</span>
                                                                        
                                                                </div>
                                                        </>
                                                )}
                                        </div>
                                )}

                                {passwordPOPUP && (
                                        <>
                                                <div className="fixed inset-0 bg-black/50 flex flex-col items-center justify-center z-50">

                                                        {error && (<div className="flex flex justify-center bg-red-300 rounded-lg w-96 p-2 m-4">
                                                                <span className="text-center text-red-800">{error}</span>
                                                        </div> )}

                                                                <div className="flex flex-col bg-amber-300 p-2 rounded-lg w-96">
                                                                        <button className="w-4 items-center" onClick={() => { setPasswordPOPUP(false); setNewParam(""); setCurrentPassword(""); setError(""); setSuccess(false);}}>
                                                                                <IoArrowBackCircleOutline size={20}/>
                                                                        </button>

                                                                        <span className="font-semibold w-full text-center block m-2 text-lg">Change your password</span>

                                                                        <span className="font-semibold m-2">Current Password</span>
                                                                        <input type="password" className="p-2 m-2 rounded-lg" value={currentPassword} onChange={(e) => {setCurrentPassword(e.target.value)}}/>

                                                                        <span className="font-semibold m-2">New Password</span>
                                                                        <input type="password" className="p-2 m-2 rounded-lg" value={newParam} onChange={(e) => {setNewParam(e.target.value)}}/>

                                                                        <button className="bg-amber-200 p-2 m-2 rounded-lg font-semibold text-gray-700" onClick={handleChangePassword}>
                                                                                Change password         
                                                                        </button>
                                                                </div>
                                                </div>
                                        </>
                                )}
                </>
        )
}

export default MyProfile;