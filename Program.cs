import os
import sys

class DistributeurTickets:
    def __init__(self):
        # Définition du chemin du dossier temp selon l'OS
        if sys.platform.startswith('win'):  # Windows
            temp_dir = os.path.join(os.environ.get('TEMP', os.path.join(os.environ['USERPROFILE'], 'temp')))
        else:  # Unix/Linux/Mac
            temp_dir = os.path.join('/tmp')
        
        # Création du dossier temp s'il n'existe pas
        try:
            if not os.path.exists(temp_dir):
                os.makedirs(temp_dir)
        except Exception as e:
            print(f"Erreur lors de la création du dossier temp: {e}")
            temp_dir = os.path.dirname(os.path.abspath(__file__))  # Utilise le dossier courant en cas d'erreur
        
        self.fichier_numeros = os.path.join(temp_dir, "fnumero.txt")
        self.compteurs = {"V": 0, "R": 0, "I": 0}
        self.clients = []
        
        # Initialisation ou lecture des compteurs existants
        self.initialiser_compteurs()
    
    def initialiser_compteurs(self):
        try:
            if not os.path.exists(self.fichier_numeros):
                # Création du fichier avec les compteurs initialisés à 0
                with open(self.fichier_numeros, "w") as f:
                    f.write("V:0\nR:0\nI:0")
            else:
                # Lecture des compteurs existants
                with open(self.fichier_numeros, "r") as f:
                    lignes = f.readlines()
                    for ligne in lignes:
                        if ":" in ligne:
                            type_op, compteur = ligne.strip().split(":")
                            if type_op in self.compteurs:
                                try:
                                    self.compteurs[type_op] = int(compteur)
                                except ValueError:
                                    self.compteurs[type_op] = 0
        except Exception as e:
            print(f"Erreur lors de l'initialisation des compteurs: {e}")
            print("Utilisation des compteurs par défaut (0)")
    
    def sauvegarder_compteurs(self):
        try:
            with open(self.fichier_numeros, "w") as f:
                for type_op, compteur in self.compteurs.items():
                    f.write(f"{type_op}:{compteur}\n")
        except Exception as e:
            print(f"Erreur lors de la sauvegarde des compteurs: {e}")
    
    def generer_ticket(self, type_operation):
        # Mapping des types d'opération à leurs codes
        codes = {"Versement": "V", "Retrait": "R", "Informations": "I"}
        
        # Incrémentation du compteur correspondant
        code = codes[type_operation]
        self.compteurs[code] += 1
        
        # Génération du numéro de ticket
        numero_ticket = f"{code}-{self.compteurs[code]}"
        
        # Sauvegarde des compteurs mis à jour
        self.sauvegarder_compteurs()
        
        return numero_ticket, self.compteurs[code] - 1  # Nombre de personnes en attente
    
    def demander_type_operation(self):
        print("\n=== Distributeur de tickets bancaires ===")
        print("Veuillez choisir le type d'opération :")
        print("1. Versement")
        print("2. Retrait")
        print("3. Informations")
        
        while True:
            try:
                choix = input("Votre choix (1-3) : ")
                if choix in ["1", "2", "3"]:
                    break
                else:
                    print("Choix invalide. Veuillez saisir 1, 2 ou 3.")
            except Exception as e:
                print(f"Erreur lors de la saisie: {e}")
        
        if choix == "1":
            return "Versement"
        elif choix == "2":
            return "Retrait"
        else:
            return "Informations"
    
    def traiter_client(self):
        try:
            # Demande du type d'opération
            type_operation = self.demander_type_operation()
            
            # Demande des informations du client
            nom = input("Veuillez saisir votre nom : ")
            prenom = input("Veuillez saisir votre prénom : ")
            numero_compte = input("Veuillez saisir votre numéro de compte : ")
            
            # Génération du ticket
            numero_ticket, attente = self.generer_ticket(type_operation)
            
            # Enregistrement du client
            self.clients.append({
                "nom": nom,
                "prenom": prenom,
                "numero_compte": numero_compte,
                "type_operation": type_operation,
                "numero_ticket": numero_ticket
            })
            
            # Affichage du message d'attente
            message = f"Votre numéro est {numero_ticket}, il y a {attente} personne(s) qui attendent avant vous."
            print("\n" + "=" * 50)
            print(message)
            print("=" * 50 + "\n")
        except Exception as e:
            print(f"Erreur lors du traitement du client: {e}")
    
    def afficher_liste_clients(self):
        print("\n=== Liste des clients ===")
        if not self.clients:
            print("Aucun client n'a pris de ticket.")
            return
            
        print("Nom\t\tPrénom\t\tType d'opération\tNuméro de ticket")
        print("-" * 70)
        for client in self.clients:
            try:
                # Assurer que le texte s'affiche correctement avec des espaces
                nom = client['nom'][:10].ljust(10)
                prenom = client['prenom'][:10].ljust(10)
                type_op = client['type_operation'].ljust(15)
                ticket = client['numero_ticket']
                print(f"{nom}\t{prenom}\t{type_op}\t{ticket}")
            except Exception as e:
                print(f"Erreur d'affichage pour un client: {e}")
    
    def executer(self):
        print("Bienvenue au distributeur automatique de tickets bancaires")
        
        continuer = True
        while continuer:
            try:
                self.traiter_client()
                
                choix = input("\nVoulez-vous prendre un autre ticket ? (O/N) : ")
                continuer = choix.upper() == "O"
            except KeyboardInterrupt:
                print("\nProgramme interrompu par l'utilisateur.")
                break
            except Exception as e:
                print(f"Une erreur s'est produite: {e}")
                choix = input("\nVoulez-vous continuer malgré l'erreur ? (O/N) : ")
                continuer = choix.upper() == "O"
        
        self.afficher_liste_clients()
        print("\nMerci d'avoir utilisé notre distributeur de tickets. À bientôt !")


# Exécution du programme
if __name__ == "__main__":
    try:
        distributeur = DistributeurTickets()
        distributeur.executer()
    except Exception as e:
        print(f"Erreur fatale: {e}")
        input("Appuyez sur Entrée pour quitter...")