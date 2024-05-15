import * as React from "react";
import Button from "@mui/material/Button";
import CssBaseline from "@mui/material/CssBaseline";
import Box from "@mui/material/Box";
import Typography from "@mui/material/Typography";
import Container from "@mui/material/Container";
import { ThemeProvider } from "@mui/material/styles";
import { defaultTheme } from "../themes";
import { createCollectedArtifact, getArtifacts, getPlayers } from "../requests";
import { useNavigate } from "react-router-dom";
import { useEffect, useState } from "react";
import { FormControl, InputLabel, MenuItem, Select } from "@mui/material";
import { displayName, CollectedArtifact, Player, Artifact } from "../database";
import { DateTimePicker, LocalizationProvider } from "@mui/x-date-pickers";
import dayjs from "dayjs";
import { AdapterDayjs } from "@mui/x-date-pickers/AdapterDayjs";
import { useAppDispatch, useAppSelector } from "../redux/hooks";
import { actions, resetState } from "../redux/slices/collected-artifact-form-slice";
import Footer from "../components/footer";

export default function CreateCollectedArtifact() {
  const navigate = useNavigate();

  const dispatch = useAppDispatch();
  const state = useAppSelector(state => state.collectedArtifactForm);

  const [players, setPlayers] = useState<Player[]>([]);
  const [artifacts, setArtifacts] = useState<Artifact[]>([]);

  useEffect(() => {
    resetState(dispatch);
    getPlayers()
      .then(players => setPlayers(players.items));
    getArtifacts()
      .then(artifacts => setArtifacts(artifacts.items));
  }, []);

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    const newCollectedArtifact: CollectedArtifact = new CollectedArtifact();

    newCollectedArtifact.playerId = state.playerId;
    newCollectedArtifact.artifactId = state.artifactId;
    newCollectedArtifact.collectionTimestamp = state.collectionTimestamp;

    const createdCollectedArtifact: CollectedArtifact | undefined = await createCollectedArtifact(newCollectedArtifact);

    if (createdCollectedArtifact) {
      navigate(`/collectedArtifact/${createdCollectedArtifact.id}`);
    }
  };

  return (
    <ThemeProvider theme={defaultTheme}>
      <Container component="main" maxWidth="xs">
        <CssBaseline />
        <Box
          sx={{
            marginTop: 10,
            display: "flex",
            flexDirection: "column",
            alignItems: "center",
          }}
        >
          <Typography component="h4" variant="h4">
            Создание подобранного артефакта
          </Typography>
          <Box component="form" onSubmit={handleSubmit} noValidate sx={{ mt: 1 }}>
            <FormControl sx={{ my: 1, minWidth: 120 }} fullWidth>
              <InputLabel id="playerLabel">Игрок</InputLabel>
              <Select
                labelId="playerLabel"
                id="player"
                label="Игрок"
                name="player"
                value={state.playerId ?? ""}
                onChange={e => dispatch(actions.setPlayerId(e.target.value))}
              >
                <MenuItem value="">
                  <em>Не выбрано</em>
                </MenuItem>
                {players.map(player => (
                  <MenuItem key={player.id} value={player.id}>{displayName(player)}</MenuItem>
                ))}
              </Select>
            </FormControl>
            <FormControl sx={{ my: 1, minWidth: 120 }} fullWidth>
              <InputLabel id="artifactLabel">Артефакт</InputLabel>
              <Select
                labelId="artifactLabel"
                id="artifact"
                label="Артефакт"
                name="artifact"
                value={state.artifactId ?? ""}
                onChange={e => dispatch(actions.setArtifactId(e.target.value))}
              >
                <MenuItem value="">
                  <em>Не выбрано</em>
                </MenuItem>
                {artifacts.map(artifact => (
                  <MenuItem key={artifact.id} value={artifact.id}>{displayName(artifact)}</MenuItem>
                ))}
              </Select>
            </FormControl>
            <LocalizationProvider dateAdapter={AdapterDayjs}>
              <DateTimePicker
                sx={{ my: 2, display: "flex" }}
                name="collectionTimestamp"
                label="Дата и время подбора"
                value={dayjs(state.collectionTimestamp)}
                onChange={value => dispatch(actions.setCollectionTimestamp(value?.toJSON() ?? ""))}
              />
            </LocalizationProvider>
            <Button
              type="submit"
              fullWidth
              variant="contained"
              color="primary"
              sx={{ mt: 3, mb: 2 }}
            >
              Создать
            </Button>
          </Box>
        </Box>
        <Footer />
      </Container>
    </ThemeProvider>
  );
}
