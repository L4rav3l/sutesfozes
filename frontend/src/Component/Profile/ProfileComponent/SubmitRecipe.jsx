import React, { useState } from "react";
import axios from "axios";

function SubmitRecipe()
{
        const [title, setTitle] = useState("");
        const [prepTime, setPrepTime] = useState();
        const [cookTime, setCookTime] = useState();
        const [serve, setServe] = useState();

        const [section, setSection] = useState([]);
        const [instructions, setInstructions] = useState("");

        const [files, setFiles] = useState([]);

        const [success, setSuccess] = useState(false);

        var apiUrl = import.meta.env.VITE_API_URL;
        var token = localStorage.getItem("token") || sessionStorage.getItem("token");

        const handleSubmit = async () => {
                try
                {
                        const response = await axios.post(`${apiUrl}/api/profile/submit_recipe`, {Title: title, PrepTime: prepTime, CookTime: cookTime, Serve: serve, Ingredients: section, Instructions: instructions}, {headers: {Authorization: `Bearer ${token}`}});
                        
                        const recipeId = response.data.id

                        for (const item of files) {
                                await handleUpload(item.file, recipeId);
                        }
                }
                catch
                {
                        alert("ERROR!");
                }
        }

        const handleUpload = async (file, id) => {
                  const formData = new FormData();
                  formData.append("File", file);
                  formData.append("Id", id);

                  try
                  {
                        const response = await axios.post(`${apiUrl}/api/profile/upload_recipe`, formData, { headers: {Authorization: `Bearer ${token}`}});
                        setSuccess(true);
                  }

                  catch
                  {
                        alert("error");
                  }
        }

        const handleCreateSection = () => {
                setSection(prev => [
                        ...prev,
                                {
                                        id: Date.now(),
                                        title: "",
                                        ingredients: []
                                }
                ]);
        };

        const handleDeleteSection = (id) => {
                setSection(prev => prev.filter(section => section.id !== id));
        };

        const handleSectionTitleChange = (id, value) => {
                setSection(prev =>
                prev.map(section =>
                section.id === id
                        ? { ...section, title: value }
                        : section
                )
                );
        };

        const handleAddIngredients = (sectionId) => {
                setSection(prev =>
                prev.map(sec =>
                sec.id === sectionId
                        ? {
                        ...sec,
                        ingredients: [
                        ...sec.ingredients,
                        {
                                id: Date.now(),
                                name: "",
                                amount: "",
                                unit: ""
                        }
                        ]
                        }
                        : sec
                )
                );
        };

        const handleIngredientChange = (sectionId, ingredientId, field, value) => {
                setSection(prev =>
                prev.map(sec =>
                sec.id === sectionId
                        ? {
                        ...sec,
                        ingredients: sec.ingredients.map(ing =>
                        ing.id === ingredientId
                                ? { ...ing, [field]: value }
                                : ing
                        )
                        }
                        : sec
                )
                );
        };

        const handleDeleteIngredient = (sectionId, ingredientId) => {
                setSection(prev =>
                prev.map(sec =>
                sec.id === sectionId
                        ? {
                        ...sec,
                        ingredients: sec.ingredients.filter(
                        ing => ing.id !== ingredientId
                        )
                        }
                        : sec
                )
                );
        };


        const handleChange = (e) => {
                const selectedFiles = Array.from(e.target.files);

                const fileData = selectedFiles.map((file) => ({
                file,
                url: URL.createObjectURL(file),
                }));

                setFiles(fileData);
        };

        console.log(section);

        return (
                <>
                {!success && (
                <div className="flex flex-col items-center m-2 h-[70vh] overflow-y-auto">
                        
                        <div className="flex flex-col m-2 items-center">
                                <span className="text-grey-700 font-semibold">Recipe name</span>
                                <input className="m-2 p-1 rounded-lg w-64" value={title} onChange={(e) => {setTitle(e.target.value)}} />
                        </div>

                        <div className="flex flex-row items-center gap-2">

                                <span className="font-semibold text-gray-700">Prep time</span>
                                <input className="p-1 rounded-lg w-16" placeholder="min" value={prepTime} onChange={(e) => setPrepTime(e.target.value) }/>

                                <span className="font-semibold text-gray-700">Cook time</span>
                                <input className="p-1 rounded-lg w-16" placeholder="min" value={cookTime} onChange={(e) => setCookTime(e.target.value)} />

                                <span className="font-semibold text-gray-700">Server</span>
                                <input className="p-1 rounded-lg w-16" placeholder="2" value={serve} onChange={(e) => setServe(e.target.value)} />

                        </div>

                        <div className="flex flex-col m-2 items-center">
                                <span className="text-grey-700 font-semibold">Ingredients</span>

                                {section.map((section) => (
                                        <div key={section.id} className="p-2 m-2 bg-amber-100 rounded-lg w-80">
                                                
                                                <input
                                                        className="p-1 w-full rounded-lg"
                                                        placeholder="Section name"
                                                        value={section.title}
                                                        onChange={(e) =>
                                                        handleSectionTitleChange(section.id, e.target.value)
                                                        }
                                                />

                                                <div className="flex items-center justify-center">

                                                        <button className="bg-green-500 m-2 p-1 rounded-lg text-gray-700 font-semibold"
                                                                onClick={() => handleAddIngredients(section.id)}
                                                        >
                                                                Add ingredients
                                                        </button>

                                                        <button
                                                                className="bg-red-500 p-1 rounded-lg m-2 text-gray-700 font-semibold"
                                                                onClick={() => handleDeleteSection(section.id)}
                                                        >
                                                                Delete section
                                                        </button>

                                                </div>

                                                {section.ingredients.map((ing) => (
                                                        <div key={ing.id} className="flex flex-col gap-2 m-2 p-2 rounded">

                                                                <input
                                                                placeholder="Ingredient name"
                                                                value={ing.name}
                                                                onChange={(e) =>
                                                                        handleIngredientChange(section.id, ing.id, "name", e.target.value)
                                                                }
                                                                className="p-1 rounded-lg text-center"
                                                                />

                                                                <input
                                                                placeholder="Amount"
                                                                value={ing.amount}
                                                                onChange={(e) =>
                                                                        handleIngredientChange(section.id, ing.id, "amount", e.target.value)
                                                                }
                                                                className="p-1 rounded-lg text-center"
                                                                />

                                                                <input
                                                                placeholder="Unit (g, ml, pcs)"
                                                                value={ing.unit}
                                                                onChange={(e) =>
                                                                        handleIngredientChange(section.id, ing.id, "unit", e.target.value)
                                                                }
                                                                className="p-1 rounded-lg text-center"
                                                                />

                                                                <button
                                                                className="bg-red-500 text-gray-700 font-semibold p-1 rounded-lg"
                                                                onClick={() =>
                                                                        handleDeleteIngredient(section.id, ing.id)
                                                                }
                                                                >
                                                                Delete
                                                                </button>

                                                        </div>
                                                ))}
                                        </div>
                                ))}
                                
                                <button className="bg-amber-300 p-2 rounded-lg m-2 text-gray-700 font-semibold" onClick={handleCreateSection}>Add sections</button>
                        </div>
                        
                        <div className="flex flex-col justify-center items-center w-full">
                                <span className="text-gray-700 font-semibold m-2">Instructions</span>

                                <textarea className="w-full h-[500px] p-3 rounded-lg" value={instructions} onChange={(e) => setInstructions(e.target.value)}/>
                        </div>

                        <div className="flex flex-col m-2 justify-center items-center ">
                                <input
                                type="file"
                                accept="image/*"
                                multiple
                                onChange={handleChange}
                                className="block w-full text-sm text-gray-500
                                        file:mr-4 file:py-2 file:px-4
                                        file:rounded-lg
                                        file:text-sm file:font-semibold
                                        file:bg-amber-50 file:text-gray-700
                                        hover:file:bg-amber-100"
                                />

                                <div className="mt-6 space-y-4">
                                {files.map((item, index) => (
                                        <div
                                                key={index}
                                                className="flex items-center gap-4 p-3 bg-amber-100 rounded-lg shadow-sm"
                                        >
                                                <img
                                                        src={item.url}
                                                        alt={item.file.name}
                                                        className="w-20 h-20 object-cover rounded-md "
                                                />

                                                <p className="text-sm text-gray-700 break-all">
                                                        {item.file.name}
                                                </p>
                                        </div>
                                ))}
                                </div>
                        </div>

                        <button className="bg-amber-100 p-2 m-2 w-64 rounded-lg" onClick={handleSubmit}>Submit</button>
                
                </div>
                )}

                {success && (
                        <div className="flex justify-center items-center">
                                <span>Your recipe has been submitted. We will accept your recipe in few days.</span>
                        </div>
                )}
                </>
        )
}

export default SubmitRecipe;