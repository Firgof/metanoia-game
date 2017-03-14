=== Shrine ===
VAR workers_present = true
VAR worker_race = "Firryn"
VAR worker_job = "worker"
VAR worker_special = "an honest-faced"
VAR met_hoenn = false

-> Entrance

=Entrance
[Short descriptor of the shrine gate]

+ [Back to the Wastes] -> Shrine.Wastes
+ [Walk through the Gate] -> Shrine.Hub

=Wastes
[Description of the wastes]

+ {WastesDescription < 2} [Look out into the wastes] -> WastesDescription
+ {WastesDescription == 2}[Look out into the wastes] -> WastesDescriptionFinal
+ [Return to the Shrine's Gate] ->Shrine.Entrance

=WastesDescription
[Drill-down of some of the history of Fyra and this place over a couple of looks]
{Wastes}
-> Shrine.Wastes

-(WastesDescriptionFinal)
[Nothing more to learn here]
-> Shrine.Wastes

=Hub
#NewPage:New
<- random_truth
{TURNS_SINCE(-> Shrine) < 8: ->WorkerScene}
{TURNS_SINCE(-> Shrine) > 7: {met_hoenn == true:->WorkerScene} {met_hoenn == false:->HoennScene}}
-(WorkerScene)
{ workers_present == true: <-describe_workers}

{ workers_present == false: The {~snow falls lazily from the clouds|moon gently reflects off the ancient snow-capped buildings, catching glints of frost buried in the white snow|air {~blows|whistles|serenades you} {~softly|gently|soothingly|eerily} as it {brushes against your {~cheek|waist|arms}|{~blows|winds|snakes|swirls} through your clothes}}.}

+ { workers_present == true} [Ask one of the workers a question.] -> WorkerConvo
+ [Study the scenery.] -> Hub

-(WorkerConvo)
<- random_worker

You call one of the workers out. After a moment's deliberation between them, {worker_special} {worker_race} {worker_job} ducks out and hurries over to you. 
-(ConvoStart)
{~"Yes?"|"Can I help you?"|"What's the matter?"}

+ {worker_job == "shrine maiden"} [What's your job here?] -> Hub
+ {worker_job == "laborer"} [What's your job here?] -> Hub
* {worker_race == "Sarin"} [You're a Sarin?] -> RaceSarin
* {worker_race == "Grynan"} [What are you?] -> RaceGrynan
+ [Nothing, thank you.] -> Hub

-(RaceGrynan)
You take stock of the towering giant in front of you. Though somewhat human in appearance this is a true titan - muscles, bulging veins, and all. Their hair sweeps back and transforms not far from the scalp into bull-like horns, from which have been embedded jewelry, teeth, and other trophies.

Despite all this, her clothing is in the Firryn style (though custom-tailored to fit her tall and wide form).  "You don't get out of Fyra much, do you little one?"

She reflexively winces at her uncouth comment about your relatively much smaller and narrower size. "Sorry, old habit." The Grynan waves the rest of the group on to continue their business.
->ConvoStart

-(RaceSarin)
VAR convo_times = 0

"Ah, haven't met a Sarin before have you? Well, what would you like to know?"  The Sarin folds her arms and looks back to wave the rest of her group onwards while you talk.

Before you stands on reverse-jointed legs a scaley-skinned lizardwoman that stands much taller than most Firryn.  A thick scaled tail wags slowly behind her and small, filed and painted, talons stretch out from her five-fingered hands.

Up from a thick serpentine neck flanked by large boney spikes a serpentine head rests, though with some strange humanoid features. Though her short snout bends up into a flat and bumpy surface her eyes are more human than snake - surrounded by scales so dense they pass for soft skin at a glance and even with eyelashes -- though she has no eyebrows; boney spines seeming to take their place instead.

Her garb is loose fitting, surpisingly, and she seems fairly comfortable in the cold snow, despite the tussling wind threatening to reveal other certain traits she shares with mammals.

-(RaceSarinConvoStart)
~convo_times = convo_times+1
{convo_times < 4:
    With a practiced, narrow, grin of her wide mouth, she cocks her head in a questioning and attentive manner.
}
{convo_times == 4:
    Her eyes flick back to where the group she waved off vanished. She bows her head respectfully. "Sorry, sir. I really should get back to work. Even as a foregin dignitary my dedicated work is expected."
    "It was a pleasure to meet you."
    With another quick bow she hurries through the snow and vanishes into the Temple.
}

    * [Aren't you cold?]
        - - Her tongue flicks out between her lips.  "Cold?"
        - - "Ah," she reaches out one of her hands and places it on your shoulder, "I am more like your kin than the snakes and lizards some have experience of."
        - - Despite their scaled nature her hands are surprisingly warm and soft. Her grip is delicate but you can feel the sharpness in her talons.
        - - She retrieves her hand. "Don't worry about my comfort." She taps a claw to a strange looking amulet hiding just under her clothes. "Brought my own heat." ->RaceSarinConvoStart
    * [Awful calm for an Intruder.]
        - - "Oh, I'm not intruding. In fact, I was invited by the King's cousin to attend this wedding. My family has ties with the Kings of Fyra that reach all the way back to the Great Shattering.
        * * [The Great Shattering?]
            - - - "Yes. Perhaps you've not heard the tale of the great weapon the Ancients built to fell the Gods? It was fired in Fyra, you know? My family - my 'Kontarrus' - supplied many of the materials needed for its construction.->RaceSarinConvoStart
        * * [Ties to the Kings of Fyra?]
            - - - "Kontarrus Sun has for time immemorial stood by the throne and my father, in fact, aided in the birth of your Second Prince. I've always wanted to see an old-fashioned Firryn wedding - and my father was owed a favor by yours - and so here I am!"
            - - - "It's so exciting!" ->RaceSarinConvoStart
    + [What are you doing?]
        - - {worker_job == "shrine_maiden":
            "Ah," she motions to the small bells, incense containers, and bands of metal draped across her wrists. "I am assisting the shrine maidens."
            "The way you celebrate the Goddess of Death is very different in Firryn culture than how Sarin celebrate her. I've learned a lot about you Firryn just from how you worship."
            - else:
            "Oh," she tugs uncertainly at some of the ropes and belted tools mounted on and wrapped her clothing. "I'm here helping the bride's family to restore the Shrine."
        } 
        - - "It's very gratifying work though I'm not yet used to the tools your kind use."->RaceSarinConvoStart
    + [Thanks for your time] 
        - - "Oh, no.  Thank you for yours!"
        - - With a quick bow, she returns to her group as they vanish into the Temple. ->Hub
            

->ConvoStart





== describe_workers ==
{~A group of|A pair of|A gaggle of|Some} {~||||sweet-smelling|herb-wafting|musky-smelling|lantern-carrying} {~shrine maidens|workers} in {~ceremonial|rope-wrapped} garb {~with well-brushed and oiled hair|wearing elaborate full-body painted designs|||} ->interruption

-(interruption)
{~do their best to be quiet as they pass by you|slip through the serene snowy scene|look around with {~suspicious|excited|inquisitive} glances|pass by}{~ ->garb|->idlechat}

-(garb)
->accent

-(idlechat)
. They {~speak in hushed tones about the groom|giggle {~about a crude joke|about the groom's mask|about their romantic pursuits|over seemingly nothing}|excitedly chat about {~you and one of them {~flashes a smile|'accidentally' brushes up against you}|the old matron|the wedding|the bride|legends and folklore surrounding the shrine|the snow-pilgrims|the Gods}} as they {~head towards a nearby structure|walk towards the temple|slip behind the rear of one of the aging structures}.->DONE

-(accent)
{~->location|->job}

-(location)
{~, take refuge under a thistled roof,|, shuffle through the snow,|, brush up against your shoulder as they pass,} ->destination

-(destination)
and {~slip behind the rear of one of the aging structures|head into a nearby structure|make their way to the temple}.->DONE

-(job)
{~haul various pieces of furniture|and take a break to rub their over-worked shoulders|and lean up against an aged wall for a moment|and take a moment to re-tie their clothing|bearing jugs of {~sweet-smelling liquid|bitter holy oil|steaming water}{~ using only their hands| on linked lengths of silken white-and-gold ropes| on an old-looking wood palette}}->idlechat

->DONE

== random_worker ==
{shuffle:
 - ~worker_special = "an honest-faced"
 - ~worker_special = "an incense-bearing"
 - ~worker_special = "a well-dressed"
 - ~worker_special = "narrow-waisted"
 - ~worker_special = "a sweet-smelling"
 - ~worker_special = "a smoke-smelling"
 - ~worker_special = "a young"
 - ~worker_special = "an older"
}
{shuffle:
 - ~worker_race = "Firryn"
 - ~worker_race = "Firryn"
 - ~worker_race = "Firryn"
 - ~worker_race = "Sarin"
 - ~worker_race = "Grynan"
}
{shuffle:
 - ~worker_job = "shrine maiden"
 - ~worker_job = "laborer"
}

== random_truth ==
{shuffle:
 - ~ workers_present = true
 - ~ workers_present = false
} ->DONE