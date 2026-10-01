using LowCarbLife.Api.Features.Recipes;
using Microsoft.EntityFrameworkCore;

namespace LowCarbLife.Api.Data;

public static class KetoSaladSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        var existing = await db.Recipes
            .Where(r => r.MealCategory == MealCategory.Salatki && r.DietType == DietType.Keto)
            .Select(r => r.Title)
            .ToListAsync();

        foreach (var recipe in CreateAll())
        {
            if (!existing.Contains(recipe.Title))
            {
                db.Recipes.Add(recipe);
            }
        }
    }

    private static IEnumerable<Recipe> CreateAll()
    {
        yield return Salad(
            "Keto sałatka z salami i jajkiem",
            "Keto sałatka na 1 porcję — rukola, salami, oliwki i jajko z sosem majonezowo-jogurtowym.",
            """
            1. Jajko ugotuj na twardo, ostudź i pokrój na ćwiartki.
            2. Paprykę pokrój w kostkę, a salami na mniejsze kawałki.
            3. Na talerzu ułóż rukolę, paprykę, oliwki, salami i jajko.
            4. Majonez wymieszaj z jogurtem, sokiem z cytryny, solą i pieprzem.
            5. Gotowym sosem polej sałatkę przed podaniem.
            """,
            "keto-salatka-salami-jajko.png",
            [
                ("Rukola", "40 g"),
                ("Czerwona papryka", "80 g"),
                ("Salami", "35 g"),
                ("Zielone oliwki", "20 g"),
                ("Czarne oliwki", "20 g"),
                ("Jajko", "1 sztuka"),
                ("Majonez", "25 g"),
                ("Jogurt grecki", "20 g"),
                ("Sok z cytryny", "1 łyżeczka"),
                ("Sól i pieprz", "do smaku")
            ]);

        yield return Salad(
            "Keto sałatka z awokado, boczkiem i serem",
            "Keto sałatka na 2 porcje — awokado, chrupiący boczek i trzy sery ze śmietankowym dressingiem.",
            """
            1. Boczek podsmaż na suchej patelni, aż będzie chrupiący.
            2. Awokado oraz sery pokrój w kostkę.
            3. Pomidorki przekrój na pół, a cebulkę drobno posiekaj.
            4. Na dnie miski ułóż miks sałat, następnie awokado, sery, boczek i pomidorki.
            5. Posyp cebulką i kolendrą.
            6. Śmietankę wymieszaj z olejem MCT i polej nią sałatkę.
            """,
            "keto-salatka-awokado-boczek-ser.png",
            [
                ("Awokado", "1 sztuka"),
                ("Boczek", "3 plastry"),
                ("Mozzarella", "50 g"),
                ("Cheddar", "50 g"),
                ("Parmezan", "50 g"),
                ("Pomidorki koktajlowe", "5 sztuk"),
                ("Cebulka dymka", "1 sztuka"),
                ("Miks sałat", "50–70 g"),
                ("Świeża kolendra", "mała garść"),
                ("Śmietanka 18%", "100 ml"),
                ("Olej MCT", "1 łyżka")
            ]);

        yield return Salad(
            "Keto sałatka z pstrągiem i szparagami",
            "Keto sałatka na 1 porcję — wędzony pstrąg, szparagi, awokado i feta na roszponce.",
            """
            1. Roszponkę umyj, osusz i ułóż na talerzu.
            2. Szparagi pozbaw twardych końcówek i gotuj na parze około 5 minut, tak aby nadal były jędrne.
            3. Pomidora i paprykę pokrój, a awokado w plasterki.
            4. Z pstrąga usuń skórę i ości, a mięso podziel na kawałki.
            5. Wszystkie składniki ułóż na roszponce, dodaj szparagi, pokruszoną fetę i polej oliwą.
            """,
            "keto-salatka-pstrag-szparagi.png",
            [
                ("Roszponka", "25 g"),
                ("Pomidor", "1 średni (ok. 60 g)"),
                ("Papryka", "½ małej (ok. 40 g)"),
                ("Awokado", "½ małego (ok. 50 g)"),
                ("Pstrąg wędzony na gorąco", "70 g"),
                ("Szparagi", "4–5 sztuk (ok. 120 g)"),
                ("Ser feta", "20 g"),
                ("Oliwa z oliwek", "4 łyżki"),
                ("Sól i pieprz", "do smaku")
            ]);

        yield return Salad(
            "Keto sałatka z indykiem i sosem orzechowym",
            "Keto sałatka na 1 dużą porcję — smażony indyk, awokado i pomidorki z sosem z masła orzechowego.",
            """
            1. Indyka pokrój w kostkę lub paski.
            2. Rozgrzej oliwę i smaż mięso około 8–10 minut.
            3. Masło orzechowe wymieszaj z sokiem z limonki, chili i sosem sojowym. Jeśli sos będzie zbyt gęsty, dodaj odrobinę wody.
            4. Sałatę przełóż do miski, dodaj przekrojone pomidorki, awokado i ciepłego indyka.
            5. Całość polej sosem orzechowym.
            """,
            "keto-salatka-indyk-sos-orzechowy.png",
            [
                ("Pierś z indyka", "200 g"),
                ("Oliwa z oliwek", "10 g"),
                ("Sałata rzymska", "20 g"),
                ("Awokado", "50 g"),
                ("Pomidorki koktajlowe", "100 g"),
                ("Masło orzechowe 100%", "20 g"),
                ("Sok z limonki", "1 łyżeczka"),
                ("Chrupiące chili w oleju", "10 g"),
                ("Sos sojowy", "20 g"),
                ("Woda", "opcjonalnie, odrobina")
            ]);

        yield return Salad(
            "Keto sałatka z boczkiem, jajkiem i cheddarem",
            "Keto sałatka na 4 porcje — sałata lodowa, chrupiący boczek, jajko i cheddar z kremowym sosem.",
            """
            1. Boczek usmaż na chrupko i pokrój.
            2. Sałatę posiekaj, ogórka, pomidorki i jajko pokrój na mniejsze kawałki.
            3. Cebulę oraz dymkę drobno posiekaj.
            4. Majonez wymieszaj ze śmietaną, octem, solą i pieprzem.
            5. W dużej misce układaj kolejno sałatę, ogórka, cebulkę, pomidorki, marchewkę, jajko i boczek.
            6. Polej sosem i posyp cheddarem. Przed jedzeniem możesz całość wymieszać.
            """,
            "keto-salatka-boczek-jajko-cheddar.png",
            [
                ("Sałata lodowa", "ok. 200 g"),
                ("Jajko ugotowane na twardo", "1 sztuka"),
                ("Usmażony boczek", "ok. 180 g"),
                ("Cebulka dymka", "1 sztuka"),
                ("Pomidorki koktajlowe", "ok. 190 g"),
                ("Marchewka", "25–30 g"),
                ("Czerwona cebula", "30 g"),
                ("Tarty cheddar", "70 g"),
                ("Ogórek", "ok. 120 g"),
                ("Majonez", "90 g"),
                ("Kwaśna śmietana", "50 g"),
                ("Ocet jabłkowy", "2 łyżeczki"),
                ("Sól i pieprz", "do smaku")
            ]);

        yield return Salad(
            "Azjatycka keto sałatka z kapustą i orzeszkami",
            "Keto sałatka z czerwoną kapustą, marchewką i orzeszkami w sezamowym dressingu. Najlepsza po schłodzeniu.",
            """
            1. Kapustę drobno poszatkuj, marchew zetrzyj, a paprykę, cebulę, dymkę i kolendrę posiekaj.
            2. Wszystko przełóż do miski.
            3. Ocet wymieszaj z olejem sezamowym, imbirem, czosnkiem, erytrytolem, solą i pieprzem.
            4. Polej sałatkę dressingiem i dokładnie wymieszaj.
            5. Odstaw na 30–45 minut do lodówki.
            6. Przed podaniem posyp orzeszkami.
            """,
            "keto-salatka-kapusta-orzeszki.png",
            [
                ("Czerwona kapusta", "350–400 g"),
                ("Marchewka", "1 średnia"),
                ("Świeża kolendra", "10 g"),
                ("Czerwona cebula", "30–40 g"),
                ("Zielona papryka", "30 g"),
                ("Cebulka dymka", "10 g"),
                ("Solone orzeszki ziemne", "15 g"),
                ("Ocet", "50 ml"),
                ("Olej sezamowy", "25 ml"),
                ("Tarty imbir", "1 łyżeczka"),
                ("Czosnek", "½ ząbka"),
                ("Erytrytol", "do smaku"),
                ("Sól i pieprz", "do smaku")
            ]);

        yield return Salad(
            "Keto sałatka ze stekiem i serem pleśniowym",
            "Keto sałatka na 2 porcje — plastry steka ribeye i kremowy sos z sera z niebieską pleśnią.",
            """
            1. Stek osusz i dopraw solą oraz pieprzem.
            2. Smaż na mocno rozgrzanej patelni po około 2–3 minuty z każdej strony lub do ulubionego stopnia wysmażenia.
            3. Odstaw na kilka minut i pokrój w cienkie plastry.
            4. Składniki sosu dokładnie wymieszaj.
            5. Sałatę połącz z pokrojonymi warzywami, na wierzchu ułóż stek i polej sosem z sera pleśniowego.
            """,
            "keto-salatka-stek-ser-plesniowy.png",
            [
                ("Stek ribeye lub antrykot", "ok. 225 g"),
                ("Miks sałat", "ok. 120 g"),
                ("Pomidorki koktajlowe", "50 g"),
                ("Ogórek", "50 g"),
                ("Czerwona papryka", "kawałek"),
                ("Czerwona cebula", "ok. 20 g"),
                ("Sól i pieprz", "do smaku"),
                ("Ser z niebieską pleśnią", "45–50 g"),
                ("Majonez", "20 g"),
                ("Kwaśna śmietana", "20 g"),
                ("Śmietanka 30%", "20 ml"),
                ("Sok z cytryny", "2 łyżeczki"),
                ("Musztarda", "½ łyżeczki"),
                ("Sos Worcestershire", "1 łyżeczka"),
                ("Czosnek", "½ ząbka"),
                ("Natka pietruszki", "do smaku")
            ]);

        yield return Salad(
            "Keto sałatka Cezar z krewetkami",
            "Keto sałatka na 4 porcje — krewetki cajun, sałata rzymska, pomidory i parmezan.",
            """
            1. Krewetki osusz i dokładnie obtocz w przyprawie cajun.
            2. Na patelni rozgrzej oliwę.
            3. Smaż krewetki około minuty z jednej strony, obróć i smaż kolejne 1–2 minuty.
            4. Sałatę posiekaj, dodaj pomidory, parmezan i sos Cezar.
            5. Wymieszaj i na wierzchu ułóż ciepłe krewetki.
            """,
            "keto-salatka-cezar-krewetki.png",
            [
                ("Obranych krewetek", "450 g"),
                ("Przyprawa cajun", "1 łyżka"),
                ("Oliwa", "2 łyżki"),
                ("Sałata rzymska", "1 duża główka"),
                ("Pomidory", "ok. 300 g"),
                ("Parmezan", "35–40 g"),
                ("Sos Cezar", "ok. 120 g")
            ]);

        yield return Salad(
            "Keto sałatka z kurczakiem i awokado",
            "Keto sałatka na 4 porcje — grillowany kurczak, jajka, awokado i dressing ziołowy.",
            """
            1. Kurczaka pokrój lub porwij na kawałki.
            2. Sałatę posiekaj, pomidora pokrój w kostkę, awokado w plasterki, a jajka na ćwiartki.
            3. Wszystko przełóż do dużej miski.
            4. Oliwę wymieszaj z octem lub sokiem z cytryny, ziołami, solą i pieprzem.
            5. Polej sałatkę dressingiem tuż przed podaniem.
            """,
            "keto-salatka-kurczak-awokado.png",
            [
                ("Ugotowany lub grillowany kurczak", "450 g"),
                ("Sałata rzymska", "2 małe główki"),
                ("Awokado", "1 sztuka"),
                ("Pomidor", "1 średni"),
                ("Jajka ugotowane na twardo", "4 sztuki"),
                ("Oliwa z oliwek", "60 ml"),
                ("Ocet z czerwonego wina lub sok z cytryny", "60 ml"),
                ("Zioła włoskie", "1–2 łyżeczki"),
                ("Sól i pieprz", "do smaku")
            ]);

        yield return Salad(
            "Keto sałatka taco z wołowiną",
            "Keto sałatka na 4 porcje — przyprawiona wołowina, cheddar, awokado, śmietana i salsa bez cukru.",
            """
            1. Wymieszaj wszystkie przyprawy.
            2. Wołowinę podsmaż na patelni, rozdrabniając ją łopatką.
            3. Dodaj przyprawy oraz około 60 ml wody i duś kilka minut, aż płyn odparuje.
            4. Sałatę przełóż do miski.
            5. Dodaj wołowinę, pomidora, awokado, cheddar, cebulę i kolendrę.
            6. Na koniec dodaj kwaśną śmietanę oraz salsę.
            """,
            "keto-salatka-taco-wolowina.png",
            [
                ("Mielona wołowina", "450 g"),
                ("Sałata rzymska", "2 małe główki"),
                ("Pomidor", "1 sztuka"),
                ("Awokado", "½ sztuki"),
                ("Tarty cheddar", "50–60 g"),
                ("Czerwona cebula", "¼ sztuki"),
                ("Świeża kolendra", "garść"),
                ("Kwaśna śmietana", "60 g"),
                ("Salsa bez dodatku cukru", "2 łyżki"),
                ("Chili w proszku", "1 łyżka"),
                ("Kmin rzymski", "1½ łyżeczki"),
                ("Sól", "1 łyżeczka"),
                ("Papryka", "½ łyżeczki"),
                ("Czosnek granulowany", "¼ łyżeczki"),
                ("Cebula granulowana", "¼ łyżeczki"),
                ("Chili lub pieprz cayenne", "szczypta"),
                ("Oregano", "¼ łyżeczki")
            ]);

        yield return Salad(
            "Grecka sałatka z kurczakiem i fetą",
            "Keto sałatka z grillowanym kurczakiem, fetą, oliwkami Kalamata i ziołowym dressingiem.",
            """
            1. Warzywa pokrój, kurczaka podziel na mniejsze kawałki i wszystko przełóż do miski.
            2. Dodaj fetę i oliwki.
            3. Składniki sosu dokładnie wymieszaj, polej nim sałatkę i gotowe.
            """,
            "keto-salatka-grecka-kurczak-feta.png",
            [
                ("Grillowany lub pieczony kurczak", "ok. 570 g"),
                ("Sałata rzymska", "1 mała główka"),
                ("Pomidor", "1 sztuka"),
                ("Papryka czerwona", "1 mała"),
                ("Ogórek", "ok. 200 g"),
                ("Czerwona cebula", "ok. 40 g"),
                ("Feta", "75 g"),
                ("Oliwki Kalamata", "45 g"),
                ("Oliwa z oliwek", "4 łyżki"),
                ("Ocet z czerwonego wina", "4 łyżki"),
                ("Sok z cytryny", "1 łyżka"),
                ("Czosnek", "2 ząbki"),
                ("Musztarda Dijon", "2 łyżeczki"),
                ("Suszony majeranek", "½ łyżeczki"),
                ("Sól i pieprz", "do smaku")
            ]);

        yield return Salad(
            "Sałatka z łososiem, awokado i szpinakiem",
            "Keto sałatka z pieczonym łososiem, szpinakiem baby i kremowym sosem koperkowym.",
            """
            1. Szpinak przełóż do miski.
            2. Dodaj kawałki łososia, pokrojone awokado, rzodkiewkę i cebulę.
            3. Wymieszaj wszystkie składniki sosu i polej nim sałatkę bezpośrednio przed podaniem.
            """,
            "keto-salatka-losos-awokado-szpinak.png",
            [
                ("Pieczony lub grillowany łosoś", "ok. 450 g"),
                ("Świeży szpinak baby", "do smaku"),
                ("Awokado", "1 sztuka"),
                ("Rzodkiewki", "4 sztuki"),
                ("Czerwona cebula", "kilka plasterków"),
                ("Majonez", "ok. 75 g"),
                ("Oliwa z oliwek", "2 łyżki"),
                ("Ocet jabłkowy", "1 łyżka"),
                ("Musztarda Dijon", "1 łyżka"),
                ("Czosnek", "2 ząbki"),
                ("Świeży koperek", "2 łyżki"),
                ("Sól i pieprz", "do smaku")
            ]);

        yield return Salad(
            "Sałatka z krewetkami i ogórkiem",
            "Chłodna keto sałatka z krewetkami, ogórkiem, koperkiem i musztardowym majonezem.",
            """
            1. Ogórka pokrój na mniejsze kawałki. Jeżeli krewetki są duże, również możesz je przekroić.
            2. Dodaj posiekaną dymkę, koperek, czosnek, majonez, sok z cytryny i musztardę.
            3. Wymieszaj i dopraw solą oraz pieprzem.
            4. Najlepiej smakuje dobrze schłodzona.
            """,
            "keto-salatka-krewetki-ogorek.png",
            [
                ("Ugotowane i obrane krewetki", "450 g"),
                ("Ogórek", "1 duży"),
                ("Cebulka dymka", "2 sztuki"),
                ("Świeży koperek", "do smaku"),
                ("Czosnek", "2 ząbki"),
                ("Majonez", "ok. 60 g"),
                ("Sok z cytryny", "2 łyżki"),
                ("Musztarda Dijon", "1 łyżka"),
                ("Sól i pieprz", "do smaku")
            ]);

        yield return Salad(
            "Tajska sałatka z wołowiną",
            "Keto sałatka z marynowanym stekiem, świeżą miętą, kolendrą i pikantnym sosem limonkowym.",
            """
            1. Wymieszaj składniki marynaty. Połowę odłóż jako sos.
            2. Wołowinę zamarynuj, a następnie usmaż na mocno rozgrzanej patelni.
            3. Po smażeniu odstaw mięso na kilka minut i pokrój w cienkie plastry.
            4. Warzywa przełóż do miski, dodaj wołowinę, świeżą miętę i kolendrę.
            5. Polej wcześniej przygotowanym sosem.
            """,
            "keto-salatka-tajska-wolowina.png",
            [
                ("Stek wołowy", "ok. 450 g"),
                ("Sałata rzymska", "do smaku"),
                ("Ogórek", "1 sztuka"),
                ("Czerwona papryka", "1 sztuka"),
                ("Czerwona cebula", "do smaku"),
                ("Świeża kolendra", "do smaku"),
                ("Świeża mięta", "do smaku"),
                ("Sok z limonki", "3 łyżki"),
                ("Oliwa", "2 łyżki"),
                ("Sos rybny", "2 łyżki"),
                ("Erytrytol", "2 łyżki"),
                ("Sriracha", "2 łyżeczki"),
                ("Czosnek", "2 ząbki"),
                ("Mielona kolendra", "do smaku"),
                ("Sól", "do smaku")
            ]);

        yield return Salad(
            "Sałatka z kurczakiem, majonezem i kolendrą",
            "Prosta keto sałatka z porwanym kurczakiem, majonezem i świeżą kolendrą.",
            """
            1. Kurczaka dopraw solą i pieprzem, a następnie usmaż na oliwie.
            2. Po usmażeniu porwij mięso przy pomocy dwóch widelców na mniejsze kawałki.
            3. Sałatę posiekaj i przełóż do miski.
            4. Dodaj kurczaka, majonez, niewielką ilość sosu Worcestershire i posiekaną kolendrę.
            5. Dopraw pieprzem i dokładnie wymieszaj.
            """,
            "keto-salatka-kurczak-majonez-kolendra.png",
            [
                ("Pierś z kurczaka", "2 sztuki"),
                ("Sałata", "do smaku"),
                ("Majonez", "do smaku"),
                ("Sos Worcestershire", "niewielka ilość"),
                ("Świeża kolendra", "do smaku"),
                ("Oliwa z oliwek", "do smażenia"),
                ("Sól", "do smaku"),
                ("Pieprz", "do smaku")
            ]);

        yield return Salad(
            "Azjatycka sałatka z wołowiną i sosem orzechowym",
            "Keto sałatka z wołowiną, grzybami enoki i kremowym sosem orzechowym.",
            """
            1. Składniki sosu dokładnie wymieszaj.
            2. Wołowinę pokrój w cienkie paski, dopraw solą i curry, a następnie usmaż na mocno rozgrzanej patelni.
            3. Zdejmij mięso, dodaj masło i podsmaż na tej samej patelni grzyby.
            4. Na talerzu ułóż sałatę, wołowinę i grzyby.
            5. Dodaj świeżą kolendrę i polej całość sosem orzechowym.
            """,
            "keto-salatka-wolowina-sos-orzechowy.png",
            [
                ("Polędwica wołowa lub stek", "do smaku"),
                ("Grzyby enoki", "do smaku"),
                ("Mix sałat", "do smaku"),
                ("Świeża kolendra", "do smaku"),
                ("Curry", "do smaku"),
                ("Masło", "ok. 1 łyżka"),
                ("Olej do smażenia", "do smaku"),
                ("Sól", "do smaku"),
                ("Czosnek", "1 ząbek"),
                ("Sos sojowy", "1 łyżeczka"),
                ("Ocet", "1 łyżeczka"),
                ("Sos rybny", "odrobina"),
                ("Sok z limonki", "do smaku"),
                ("Masło orzechowe 100%", "1 łyżka"),
                ("Oliwa", "2 łyżki"),
                ("Pieprz", "do smaku")
            ]);

        yield return Salad(
            "Caprese z mozzarellą i pomidorkami",
            "Prosta keto sałatka caprese — mozzarella, pomidorki koktajlowe i świeża bazylia.",
            """
            1. Mozzarellę i pomidorki przekrój na pół. Bazylię pokrój w cienkie paski.
            2. Oliwę wymieszaj z octem balsamicznym i solą.
            3. Dodaj pomidorki oraz mozzarellę i delikatnie wymieszaj.
            4. Na koniec dodaj świeżą bazylię.
            """,
            "keto-salatka-caprese.png",
            [
                ("Małe kulki mozzarelli", "do smaku"),
                ("Pomidorki koktajlowe", "do smaku"),
                ("Świeża bazylia", "do smaku"),
                ("Oliwa extra virgin", "1 łyżka"),
                ("Ocet balsamiczny", "niewielka ilość"),
                ("Sól", "do smaku")
            ]);

        yield return Salad(
            "Klasyczna sałatka grecka",
            "Keto wersja klasycznej sałatki greckiej z fetą, oliwkami i oregano.",
            """
            1. Ogórka, pomidorki, cebulę i paprykę pokrój na większe kawałki.
            2. Oliwę wymieszaj z octem, oregano, solą i pieprzem.
            3. Warzywa przełóż do miski i polej dressingiem.
            4. Na wierzchu dodaj oliwki Kalamata i pokruszoną fetę.
            5. Posyp dodatkową porcją oregano.
            """,
            "keto-salatka-grecka-klasyczna.png",
            [
                ("Ogórek", "do smaku"),
                ("Pomidorki koktajlowe", "do smaku"),
                ("Czerwona cebula", "do smaku"),
                ("Zielona papryka", "do smaku"),
                ("Oliwki Kalamata", "do smaku"),
                ("Feta", "do smaku"),
                ("Oregano", "do smaku"),
                ("Oliwa z oliwek", "2 łyżki"),
                ("Ocet z czerwonego wina", "1 łyżka"),
                ("Sól i pieprz", "do smaku")
            ]);

        yield return Salad(
            "Sałatka z kurczakiem i kremowym sosem orzechowym",
            "Keto sałatka z udkami curry, mixem sałat i kremowym sosem z masła orzechowego.",
            """
            1. Kurczaka dopraw solą, pieprzem i curry.
            2. Usmaż około 3–4 minuty z każdej strony, a następnie pokrój na kawałki.
            3. Masło orzechowe wymieszaj z wodą. Dodaj sok z cytryny lub limonki, sos sojowy, oliwę, sól i pieprz.
            4. Na talerzu ułóż mix sałat i pomidorki.
            5. Polej sosem, posyp sezamem, dodaj kurczaka i świeżą kolendrę.
            """,
            "keto-salatka-kurczak-kremowy-sos-orzechowy.png",
            [
                ("Udka z kurczaka bez kości i skóry", "2 sztuki"),
                ("Mix sałat", "do smaku"),
                ("Pomidorki koktajlowe", "do smaku"),
                ("Biały i czarny sezam", "do smaku"),
                ("Świeża kolendra", "do smaku"),
                ("Oliwa", "do smażenia"),
                ("Curry", "do smaku"),
                ("Sól i pieprz", "do smaku"),
                ("Masło orzechowe 100%", "1 łyżka"),
                ("Woda", "1 łyżka"),
                ("Sok z cytryny lub limonki", "do smaku"),
                ("Sos sojowy", "ok. 1 łyżeczka"),
                ("Oliwa do sosu", "1 łyżka"),
                ("Stewia", "opcjonalnie, odrobina")
            ]);

        yield return Salad(
            "Sałatka Cezar z kurczakiem i chrupiącym boczkiem",
            "Keto sałatka na 1 dużą porcję — kurczak, boczek, parmezan i klasyczny sos Cezar.",
            """
            1. Kurczaka dopraw solą i pieprzem, skrop oliwą i usmaż na patelni po około 5–6 minut z każdej strony. Następnie pokrój w plastry.
            2. Boczek usmaż bez dodatkowego tłuszczu, aż będzie chrupiący.
            3. Wszystkie składniki sosu dokładnie wymieszaj lub zblenduj.
            4. Sałatę porwij na mniejsze kawałki i przełóż do miski.
            5. Dodaj kurczaka, pokruszony boczek i parmezan. Polej sosem tuż przed podaniem.
            """,
            "keto-salatka-cezar-kurczak-boczek.png",
            [
                ("Pierś z kurczaka", "175 g"),
                ("Boczek", "40 g (ok. 3 plastry)"),
                ("Sałata rzymska", "100 g (ok. ½ główki)"),
                ("Parmezan", "30 g (ok. 3 łyżki)"),
                ("Oliwa", "½ łyżki"),
                ("Sól i pieprz", "do smaku"),
                ("Majonez", "50 g (ok. 2 pełne łyżki)"),
                ("Musztarda Dijon", "½ łyżki"),
                ("Sok z cytryny", "1 łyżka"),
                ("Parmezan do sosu", "10 g (ok. 1 łyżka)"),
                ("Filecik anchois", "1 sztuka, opcjonalnie"),
                ("Czosnek", "½ ząbka")
            ],
            850);

        yield return Salad(
            "Sałatka z jajkiem, boczkiem i serem pleśniowym",
            "Keto sałatka na 1 porcję — jajko, chrupiący boczek i ser pleśniowy z lekkim sosem śmietanowym.",
            """
            1. Jajko ugotuj na twardo, obierz i pokrój na ćwiartki.
            2. Boczek pokrój i usmaż na chrupko.
            3. Majonez połącz ze śmietaną, sokiem z cytryny, czosnkiem, solą i pieprzem.
            4. Sałatę pokrój na większe kawałki i ułóż na talerzu.
            5. Dodaj pomidora, jajko, boczek i pokruszony ser pleśniowy.
            6. Całość polej przygotowanym sosem.
            """,
            "keto-salatka-jajko-boczek-ser-plesniowy.png",
            [
                ("Sałata lodowa", "120 g (ok. ¼ małej główki)"),
                ("Boczek", "55 g (ok. 4 plastry)"),
                ("Jajko", "1 duża sztuka"),
                ("Pomidor", "½ sztuki (ok. 60 g)"),
                ("Ser pleśniowy", "30 g"),
                ("Majonez", "20 g (ok. 1 pełna łyżka)"),
                ("Śmietana 18%", "15 g (ok. 1 łyżka)"),
                ("Sok z cytryny", "1 łyżeczka"),
                ("Czosnek granulowany", "szczypta"),
                ("Sól i pieprz", "do smaku")
            ],
            620);

        yield return Salad(
            "Sałatka cheeseburgerowa z wołowiną i cheddarem",
            "Keto sałatka na 1 porcję — smak cheeseburgera bez bułki, z gorącą wołowiną i cheddarem.",
            """
            1. Na patelni rozgrzej masło i dodaj mięso mielone.
            2. Dopraw czosnkiem, solą i pieprzem. Smaż około 7–8 minut, rozdrabniając mięso łopatką.
            3. Wymieszaj wszystkie składniki sosu.
            4. Sałatę przełóż do miski. Dodaj pomidora, ogórka konserwowego i cebulę.
            5. Na warzywach ułóż gorącą wołowinę i posyp startym cheddarem.
            6. Polej sosem i podawaj od razu.
            """,
            "keto-salatka-cheeseburgerowa.png",
            [
                ("Mielona wołowina", "160 g"),
                ("Masło", "½ łyżki"),
                ("Cheddar", "30 g"),
                ("Sałata", "50 g (ok. 2 garście)"),
                ("Pomidor", "½ sztuki (ok. 60 g)"),
                ("Ogórek konserwowy", "1 średnia sztuka"),
                ("Czerwona cebula", "15 g"),
                ("Czosnek granulowany", "½ łyżeczki"),
                ("Sól i pieprz", "do smaku"),
                ("Majonez", "30 g (ok. 1½ łyżki)"),
                ("Musztarda", "1 łyżeczka"),
                ("Ocet winny lub jabłkowy", "½ łyżeczki"),
                ("Drobno posiekany ogórek konserwowy", "1 łyżka")
            ],
            750);

        yield return Salad(
            "Pikantna sałatka z krewetkami i awokado",
            "Keto sałatka na 1 porcję — krewetki z chili, awokado i sos imbirowy.",
            """
            1. Składniki sosu dokładnie wymieszaj.
            2. Awokado pokrój w kostkę, skrop sokiem z limonki i lekko posól.
            3. Na patelni rozgrzej oliwę. Dodaj czosnek, chili oraz krewetki.
            4. Smaż krewetki po około 2 minuty z każdej strony.
            5. W misce ułóż szpinak, ogórka i awokado.
            6. Dodaj ciepłe krewetki, polej sosem i posyp posiekanymi orzechami.
            """,
            "keto-salatka-krewetki-awokado.png",
            [
                ("Krewetki obrane", "140 g"),
                ("Awokado", "½ dużej sztuki (ok. 100 g)"),
                ("Ogórek", "70 g (ok. ⅓ sztuki)"),
                ("Szpinak baby", "30 g (ok. 2 garście)"),
                ("Oliwa", "½ łyżki"),
                ("Czosnek", "½ ząbka"),
                ("Chili", "½ łyżeczki"),
                ("Orzechy laskowe lub ziemne", "10 g (ok. 1 łyżka)"),
                ("Sól i pieprz", "do smaku"),
                ("Oliwa lub olej z awokado", "2 łyżki"),
                ("Świeży imbir", "½ łyżki"),
                ("Sok z limonki", "1 łyżka"),
                ("Czosnek do sosu", "½ ząbka"),
                ("Sos sojowy", "1 łyżeczka")
            ],
            580);

        yield return Salad(
            "Sałatka taco z wołowiną i guacamole",
            "Keto sałatka na 1 porcję — wołowina taco, guacamole, cheddar i sos z salsy.",
            """
            1. Na oliwie podsmaż mięso mielone.
            2. Dodaj przyprawę do taco i wodę. Duś do momentu, aż płyn odparuje.
            3. Awokado rozgnieć widelcem. Dodaj czosnek, sok z limonki, natkę, sól i chili.
            4. Majonez wymieszaj z salsą.
            5. Do miski dodaj sałatę, ogórka, pomidora i cebulę.
            6. Na warzywach ułóż mięso, guacamole i starty cheddar. Całość polej sosem.
            """,
            "keto-salatka-taco-guacamole.png",
            [
                ("Mielona wołowina", "200 g"),
                ("Oliwa", "½ łyżki"),
                ("Przyprawa do taco", "½ łyżki"),
                ("Woda", "45 ml (ok. 3 łyżki)"),
                ("Sałata", "40 g (ok. 2 garście)"),
                ("Ogórek", "40 g"),
                ("Pomidor", "30 g"),
                ("Czerwona cebula", "15 g"),
                ("Cheddar", "15 g"),
                ("Sól i pieprz", "do smaku"),
                ("Awokado", "½ dużej sztuki (ok. 100 g)"),
                ("Czosnek", "½ ząbka"),
                ("Sok z limonki", "1 łyżeczka"),
                ("Posiekana natka pietruszki lub kolendra", "½ łyżki"),
                ("Chili", "opcjonalnie"),
                ("Majonez", "30 g (ok. 1½ łyżki)"),
                ("Salsa pomidorowa bez dodatku cukru", "1 łyżka")
            ],
            850);

        yield return Salad(
            "Sałatka kalafiorowa z boczkiem i sosem musztardowym",
            "Keto sałatka na 1 porcję — kalafior na parze, boczek i musztardowy majonez. Podawaj schłodzoną.",
            """
            1. Kalafior podziel na małe różyczki i ugotuj na parze przez około 7–8 minut. Powinien zmięknąć, ale nadal pozostać lekko chrupiący.
            2. Boczek pokrój i usmaż na chrupko.
            3. Seler i cebulę drobno posiekaj.
            4. Majonez wymieszaj z musztardą i octem jabłkowym.
            5. Przestudzony kalafior połącz z boczkiem, selerem, cebulą oraz szczypiorkiem.
            6. Dodaj sos, wymieszaj i włóż na około 20 minut do lodówki.
            """,
            "keto-salatka-kalafior-boczek.png",
            [
                ("Kalafior", "200 g"),
                ("Boczek", "50 g (ok. 4 plastry)"),
                ("Seler naciowy", "1 mała łodyga (ok. 40 g)"),
                ("Czerwona cebula", "15 g"),
                ("Szczypiorek", "1 łyżka"),
                ("Sól i pieprz", "do smaku"),
                ("Majonez", "40 g (ok. 2 łyżki)"),
                ("Musztarda Dijon", "1 łyżeczka"),
                ("Ocet jabłkowy", "1 łyżeczka")
            ],
            510);

        yield return Salad(
            "Sałatka BLT z kurczakiem i sosem czosnkowym",
            "Keto sałatka na 1 porcję — kurczak, chrupiący boczek, pomidorki i sos czosnkowy.",
            """
            1. Majonez wymieszaj z czosnkiem i sokiem z cytryny.
            2. Boczek usmaż na chrupko i zdejmij z patelni.
            3. Kurczaka dopraw, pokrój na kawałki i usmaż na tłuszczu wytopionym z boczku.
            4. Sałatę porwij, a pomidorki przekrój na pół.
            5. Do miski dodaj sałatę, kurczaka, boczek i pomidorki.
            6. Całość polej sosem czosnkowym.
            """,
            "keto-salatka-blt-kurczak.png",
            [
                ("Udko z kurczaka bez kości i skóry", "120 g"),
                ("Boczek", "55 g (ok. 4 plastry)"),
                ("Sałata rzymska lub lodowa", "70 g"),
                ("Pomidorki koktajlowe", "30 g (ok. 3 sztuki)"),
                ("Masło", "1 łyżeczka, opcjonalnie"),
                ("Sól i pieprz", "do smaku"),
                ("Majonez", "35 g (ok. 1½ łyżki)"),
                ("Czosnek granulowany", "½ łyżeczki"),
                ("Sok z cytryny", "1 łyżeczka")
            ],
            690);

        yield return Salad(
            "Sałatka z łososiem, krewetkami i awokado",
            "Keto sałatka na 1 porcję — łosoś, krewetki, awokado i kremowy sos limonkowy.",
            """
            1. Łososia podziel na mniejsze kawałki, a krewetki przekrój na pół.
            2. Awokado, ogórka i pomidora pokrój w kostkę. Cebulę drobno posiekaj.
            3. Majonez połącz ze śmietaną, sokiem z limonki i przeciśniętym czosnkiem.
            4. Wszystkie składniki przełóż do miski i delikatnie wymieszaj.
            5. Dodaj sos oraz świeżą bazylię. Dopraw do smaku.
            """,
            "keto-salatka-losos-krewetki-awokado.png",
            [
                ("Ugotowane lub usmażone krewetki", "100 g"),
                ("Pieczony łosoś", "100 g"),
                ("Awokado", "⅓ dużej sztuki (ok. 70 g)"),
                ("Ogórek", "50 g"),
                ("Pomidor", "50 g"),
                ("Czerwona cebula", "10 g"),
                ("Świeża bazylia", "1 łyżka"),
                ("Majonez", "20 g (ok. 1 łyżka)"),
                ("Śmietana 18%", "15 g (ok. 1 łyżka)"),
                ("Sok z limonki", "1 łyżka"),
                ("Czosnek", "½ ząbka"),
                ("Sól i pieprz", "do smaku")
            ],
            570);

        yield return Salad(
            "Sałatka z łososiem, awokado i pestkami",
            "Keto sałatka na 1 porcję — smażony łosoś, awokado, oliwki i sos musztardowo-kaparowy.",
            """
            1. Łososia dopraw solą i pieprzem. Usmaż na oliwie przez około 4 minuty z każdej strony.
            2. Ogórka i awokado pokrój w kostkę. Groszek cukrowy pokrój w cienkie paski.
            3. Dodaj oliwki, suszone pomidory oraz posiekaną zieleninę.
            4. Składniki sosu wymieszaj w osobnym naczyniu.
            5. Na sałatce ułóż kawałki ciepłego łososia.
            6. Polej sosem i posyp pestkami dyni oraz słonecznika.
            """,
            "keto-salatka-losos-awokado-pestki.png",
            [
                ("Filet z łososia", "130 g"),
                ("Awokado", "½ sztuki (ok. 70 g)"),
                ("Ogórek", "100 g"),
                ("Groszek cukrowy", "30 g"),
                ("Oliwki", "15 g (ok. 5 sztuk)"),
                ("Suszone pomidory", "10 g"),
                ("Pestki dyni", "½ łyżki"),
                ("Pestki słonecznika", "½ łyżki"),
                ("Świeży koperek lub natka pietruszki", "1 łyżka"),
                ("Oliwa do smażenia", "1 łyżeczka"),
                ("Sól i pieprz", "do smaku"),
                ("Oliwa", "1½ łyżki"),
                ("Musztarda Dijon", "½ łyżki"),
                ("Kapary", "½ łyżki"),
                ("Sok z cytryny lub ocet jabłkowy", "½ łyżki"),
                ("Sos sojowy", "½ łyżeczki")
            ],
            710);
    }

    private static Recipe Salad(
        string title,
        string description,
        string instructions,
        string imageFile,
        (string Name, string Amount)[] ingredients,
        decimal? calories = null)
    {
        var recipe = new Recipe
        {
            Id = Guid.NewGuid(),
            Title = title,
            Description = description,
            Instructions = instructions,
            ImageUrl = $"/recipes/{imageFile}",
            DietType = DietType.Keto,
            MealCategory = MealCategory.Salatki,
            CaloriesKcal = calories,
            CreatedAt = DateTime.UtcNow,
        };

        for (var i = 0; i < ingredients.Length; i++)
        {
            recipe.Ingredients.Add(new RecipeIngredient
            {
                Id = Guid.NewGuid(),
                Name = ingredients[i].Name,
                Amount = ingredients[i].Amount,
                SortOrder = i
            });
        }

        return recipe;
    }
}
