/*
 * Copyright 2017 Open University of the Netherlands (OUNL)
 *
 * Authors: Kiavash Bahreini, Wim van der Vegt.
 * Organization: Open University of the Netherlands (OUNL).
 * Project: The RAGE project
 * Project URL: http://rageproject.eu.
 * Task: T2.3 of the RAGE project; Development of assets for emotion detection. 
 * 
 * For any questions please contact: 
 *
 * Kiavash Bahreini via kiavash.bahreini [AT] ou [DOT] nl
 * and/or
 * Wim van der Vegt via wim.vandervegt [AT] ou [DOT] nl
 *
 * Cite this work as:
 * Bahreini, K., van der Vegt, W. & Westera, W. Multimedia Tools and Applications (2019). https://doi.org/10.1007/s11042-019-7250-z
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * This project has received funding from the European Union’s Horizon
 * 2020 research and innovation programme under grant agreement No 644187.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace dlib_csharp
{
    using System;
    using System.Windows.Forms;

    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
