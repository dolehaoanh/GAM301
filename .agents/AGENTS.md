# Workspace Rules

- Unity script code generated in this project will not have Header descriptions (such as `[Header("Movement Settings")]`).
- If an implementation plan requires no automation or steps automatically done by the agent, the agent does not need to create or return an `implementation_plan.md` artifact; instead, include all the content directly in the chat response.
- Unity script variables that would normally be serialized using `[SerializeField]` will be declared as `public` variables instead.
- Do not generate comments in the generated code or scripts unless explicitly asked to do so.
- Name all variables, methods, and class names in Vietnamese without diacritics (tiếng Việt không dấu) using standard naming conventions (e.g. PascalCase for classes/methods, camelCase for variables/fields).
- C# script files created by the agent should also be named in Vietnamese without diacritics (tiếng Việt không dấu).




