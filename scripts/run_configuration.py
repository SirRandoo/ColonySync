from pathlib import Path
from xml.etree import ElementTree as ET

from environment import Environment


def generate_run_configuration():
    """
    Generates a run configuration XML file for launching RimWorld using a Unity Executable.

    The function initializes an environment instance and creates the root XML element
    with the name "ProjectRunConfigurationManager". It sets various options related to
    the executable path, program parameters, working directory, parent environment
    inheritance, and console usage.

    Also, it defines method elements to be executed as part of the run configuration,
    specifying multiple Python tasks such as "Un-Nest Assemblies", "Consolidate Assemblies",
    "Deploy StreamKit", and "Ensure Active". Finally, it writes the generated XML
    structure to a file named "Launch RimWorld.run.xml" into the ".run" directory.
    """
    environment = Environment.create_instance()

    root_element: ET.Element = ET.Element(
        "component", attrib={"name": "ProjectRunConfigurationManager"}
    )

    configuration_element: ET.Element = ET.Element(
        "configuration",
        attrib={
            "default": "false",
            "name": "Launch RimWorld",
            "type": "RunUnityExe",
            "factoryName": "Unity Executable",
        },
    )

    ET.SubElement(
        configuration_element,
        "option",
        attrib={
            "name": "EXE_PATH",
            "value": str(environment.game_install_path.joinpath("RimWorldWin64.exe")),
        },
    )

    ET.SubElement(
        configuration_element,
        "option",
        attrib={"name": "PROGRAM_PARAMETERS", "value": "-logFile -"},
    )

    ET.SubElement(
        configuration_element,
        "option",
        attrib={
            "name": "WORKING_DIRECTORY",
            "value": str(environment.game_install_path),
        },
    )
    ET.SubElement(
        configuration_element,
        "option",
        attrib={"name": "PASS_PARENT_ENVS", "value": "1"},
    )
    ET.SubElement(
        configuration_element,
        "option",
        attrib={"name": "USE_EXTERNAL_CONSOLE", "value": "0"},
    )

    method_element = ET.SubElement(configuration_element, "method", attrib={"v": "2"})
    ET.SubElement(
        method_element,
        "option",
        attrib={
            "name": "RunConfigurationTask",
            "enabled": "true",
            "run_configuration_name": "Un-Nest Assemblies",
            "run_configuration_type": "PythonConfigurationType",
        },
    )

    ET.SubElement(
        method_element,
        "option",
        attrib={
            "name": "RunConfigurationTask",
            "enabled": "true",
            "run_configuration_name": "Consolidate Assemblies",
            "run_configuration_type": "PythonConfigurationType",
        },
    )

    ET.SubElement(
        method_element,
        "option",
        attrib={
            "name": "RunConfigurationTask",
            "enabled": "true",
            "run_configuration_name": "Deploy StreamKit",
            "run_configuration_type": "PythonConfigurationType",
        },
    )

    ET.SubElement(
        method_element,
        "option",
        attrib={
            "name": "RunConfigurationTask",
            "enabled": "true",
            "run_configuration_name": "Ensure Active",
            "run_configuration_type": "PythonConfigurationType",
        },
    )

    with Path(".run/Launch RimWorld.run.xml").open("w") as f:
        ET.ElementTree(root_element).write(f, encoding="utf-8")
