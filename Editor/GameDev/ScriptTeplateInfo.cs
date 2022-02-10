using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.GameDev
{
    public static class ScriptTeplateInfo
    {
        public static readonly string cpp_file = "#include " + '"' + "{0}.h" + '"' + '\n' +
            "namespace {1} {{\n"
            + "\tREGISTER_SCRIPT({0});\n"
            + "\tvoid {0}::Begin()\n"
            + "\t{{\n"
            + "\t\t\n"
            + "\t}}\n"
            + "\tvoid {0}::Update(float dt)\n"
            + "\t{{\n"
            + "\t\t\n"
            + "\t}}\n"
            +"}}";
        public static readonly string h_file = "#pragma once\n\n"
            + "namespace {1}"
            + "{{\n"
            + "\tclass {0} :public RedHandEngine::script::entity_script {{\n"
            + "public:\n"
            + "\tconstexpr explicit {0}(RedHandEngine::game_entity::entity entity):RedHandEngine::script::entity_script(entity){{}}\n\n"
            + "\tvoid Begin() override;\n"
            + "\tvoid Update(float dt) override;\n"
            + "\t}};\n"
            + "}}";
    }
}
