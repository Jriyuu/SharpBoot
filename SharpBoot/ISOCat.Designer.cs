﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Ce code a été généré par un outil.
//     Version du runtime :4.0.30319.42000
//
//     Les modifications apportées à ce fichier peuvent provoquer un comportement incorrect et seront perdues si
//     le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace SharpBoot {
    /// <summary>
    ///   Une classe de ressource fortement typée destinée, entre autres, à la consultation des chaînes localisées.
    /// </summary>
    // Cette classe a été générée automatiquement par la classe StronglyTypedResourceBuilder
    // à l'aide d'un outil, tel que ResGen ou Visual Studio.
    // Pour ajouter ou supprimer un membre, modifiez votre fichier .ResX, puis réexécutez ResGen
    // avec l'option /str ou régénérez votre projet VS.
    [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [DebuggerNonUserCode()]
    [CompilerGenerated()]
    internal class ISOCat {
        
        private static ResourceManager resourceMan;
        
        private static CultureInfo resourceCulture;
        
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ISOCat() {
        }
        
        /// <summary>
        ///   Retourne l'instance ResourceManager mise en cache utilisée par cette classe.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static ResourceManager ResourceManager {
            get {
                if (ReferenceEquals(resourceMan, null)) {
                    ResourceManager temp = new ResourceManager("SharpBoot.ISOCat", typeof(ISOCat).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Remplace la propriété CurrentUICulture du thread actuel pour toutes
        ///   les recherches de ressources à l'aide de cette classe de ressource fortement typée.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Backup.
        /// </summary>
        internal static string Backup {
            get {
                return ResourceManager.GetString("Backup", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à BIOS.
        /// </summary>
        internal static string Bios {
            get {
                return ResourceManager.GetString("Bios", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à CPU.
        /// </summary>
        internal static string CPU {
            get {
                return ResourceManager.GetString("CPU", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Linux.
        /// </summary>
        internal static string Linux {
            get {
                return ResourceManager.GetString("Linux", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Partition.
        /// </summary>
        internal static string Partition {
            get {
                return ResourceManager.GetString("Partition", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Password.
        /// </summary>
        internal static string Password {
            get {
                return ResourceManager.GetString("Password", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à RAM.
        /// </summary>
        internal static string RAM {
            get {
                return ResourceManager.GetString("RAM", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Recovery.
        /// </summary>
        internal static string Recovery {
            get {
                return ResourceManager.GetString("Recovery", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Testing.
        /// </summary>
        internal static string Testing {
            get {
                return ResourceManager.GetString("Testing", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Utility.
        /// </summary>
        internal static string Utility {
            get {
                return ResourceManager.GetString("Utility", resourceCulture);
            }
        }
    }
}
