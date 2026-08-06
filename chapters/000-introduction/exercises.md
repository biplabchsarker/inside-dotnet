# Exercises — Welcome to Inside .NET

These exercises are deliberately lightweight — this chapter has no code to run yet, so the goal is to prime your own curiosity before Episode 2 supplies the mechanism.

1. **Name your unexamined assumption.**
   Pick one .NET behavior you rely on every day but have never actually verified — for example, "objects I don't reference get garbage collected eventually," or "`async`/`await` doesn't block a thread," or "a `singleton` DI registration means exactly one instance for the app's lifetime." Write down, in your own words, your best guess at *why* it works that way — before reading any further chapter. You'll revisit this note when the relevant chapter arrives and check how close your mental model was.

2. **Find one production incident you've lived through.**
   Think of a real bug or outage you debugged (or watched someone else debug) that turned out to be caused by a misunderstanding of runtime behavior — a memory leak, a deadlock, a slow endpoint that turned out to be an N+1 query, a DI lifetime mistake. Write two or three sentences: what the symptom looked like from the outside, and what the actual root cause turned out to be once someone understood the mechanism. Keep this note — several later chapters in this series will give you the vocabulary to explain *why* that incident happened, not just *that* it happened.

3. **Map your own learning gaps against the eleven-part structure.**
   Look at the table in this chapter's [Visual Explanation](article.md#visual-explanation) section. For each of the eleven parts, rate your own confidence from 1 (never really understood it) to 5 (could explain it to a junior in an interview setting). Don't fix anything yet — just have the map. Revisit it after Part II (Memory) and see which rating moved the most.
